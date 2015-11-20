using UnityEditor;
using UnityEngine;
using System;

namespace PygmyMonkey.AdvancedBuilder
{
	[Serializable]
	public class TextureCompressionBlackBerry : ITextureCompression
	{
		/*
		 * Platform common properties
		 */
		[SerializeField] private TextureProperties m_textureProperties;
		
		public TextureProperties getTextureProperties()
		{
			return m_textureProperties;
		}
		
		
		/*
		 * Return the BlackBerry Build Sub Target
		 */
		public readonly MobileTextureSubtarget subTarget;
		

		/*
		 * Constructor
		 */
		public TextureCompressionBlackBerry(MobileTextureSubtarget subTarget, bool isActive = false)
		{
			this.subTarget = subTarget;

			m_textureProperties = new TextureProperties(subTarget.ToString(), isActive);
		}
	}
}
