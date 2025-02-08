using System.Text;

namespace Scarlet.Core
{
	public static class Encoding
	{
		/// <summary>
		/// 기본적으로 UTF-8 no-bom 인코딩 사용.
		/// </summary>
		public static System.Text.Encoding Default { get; } = new UTF8Encoding(false);
	}
}
