using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Frozen
{
	class DefaultContent
	{
		private readonly ContentManager contentManager;
		private readonly Assembly assembly;
		private readonly Dictionary<string, string> availableResources;

		private readonly MethodInfo GetContentReaderFromXnb;
		private readonly MethodInfo ReadAsset;

		internal DefaultContent(ContentManager contentManager)
		{
			this.contentManager = contentManager;
			this.assembly = Assembly.GetExecutingAssembly();
			this.availableResources = this.assembly
				.GetManifestResourceNames()
				.ToDictionary(k => this.ExtractResourceKey(k), v => v);

			// Hacking in MonoGame's Content reader
			this.GetContentReaderFromXnb = typeof(ContentManager)
				.GetMethod(nameof(this.GetContentReaderFromXnb), BindingFlags.Instance | BindingFlags.NonPublic);
			this.ReadAsset = typeof(ContentReader)
				.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
				.First(m => m.Name == nameof(this.ReadAsset) && m.IsGenericMethodDefinition);
		}

		public T Get<T>(string assetName)
		{
			T result = default;
			if (this.availableResources.TryGetValue(assetName, out string resourceKey))
			{
				Stream stream = this.assembly.GetManifestResourceStream(resourceKey);

				using (BinaryReader xnbReader = new BinaryReader(stream))
				{
					using (ContentReader reader = this.GetContentReader(assetName, stream, xnbReader))
					{
						result = (T)this.ReadAsset.MakeGenericMethod(typeof(T)).Invoke(reader, null);
						if (result is GraphicsResource gr)
							gr.Name = assetName;
					}
				}
			}
			return result;
		}

		private string ExtractResourceKey(string resourceName)
		{
			return string.Join(".", resourceName.Split('.').Reverse().Take(2).Reverse());
		}

		private ContentReader GetContentReader(string assetName, Stream stream, BinaryReader xnbReader)
		{
			return this.GetContentReaderFromXnb.Invoke(this.contentManager, new object[] { assetName, stream, xnbReader, null }) as ContentReader;
		}
	}
}
