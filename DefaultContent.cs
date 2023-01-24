using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Frozen
{
	internal class DefaultContent
	{
		private readonly Assembly _assembly;
		private readonly Dictionary<string, string> _availableResources;
		private readonly ContentManager _contentManager;

		private readonly MethodInfo GetContentReaderFromXnb;
		private readonly MethodInfo ReadAsset;

		internal DefaultContent(ContentManager contentManager)
		{
			_contentManager = contentManager;
			_assembly = Assembly.GetExecutingAssembly();
			_availableResources = _assembly
				.GetManifestResourceNames()
				.ToDictionary(k => ExtractResourceKey(k), v => v);

			// Hacking in MonoGame's Content reader
			GetContentReaderFromXnb = typeof(ContentManager)
				.GetMethod(nameof(GetContentReaderFromXnb), BindingFlags.Instance | BindingFlags.NonPublic);
			ReadAsset = typeof(ContentReader)
				.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
				.First(m => m.Name == nameof(ReadAsset) && m.IsGenericMethodDefinition);
		}

		private string ExtractResourceKey(string resourceName)
		{
			return string.Join(".", resourceName.Split('.').Reverse().Take(2).Reverse());
		}

		private ContentReader GetContentReader(string assetName, Stream stream, BinaryReader xnbReader)
		{
			return GetContentReaderFromXnb.Invoke(_contentManager, new object[] { assetName, stream, xnbReader, null }) as ContentReader;
		}

		public T Get<T>(string assetName)
		{
			T result = default;
			if (_availableResources.TryGetValue(assetName, out string resourceKey))
			{
				Stream stream = _assembly.GetManifestResourceStream(resourceKey);

				using (BinaryReader xnbReader = new BinaryReader(stream))
				{
					using (ContentReader reader = GetContentReader(assetName, stream, xnbReader))
					{
						result = (T)ReadAsset.MakeGenericMethod(typeof(T)).Invoke(reader, null);
						if (result is GraphicsResource gr)
							gr.Name = assetName;
					}
				}
			}
			return result;
		}
	}
}
