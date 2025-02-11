using System.Globalization;

public static class GlobalizationUtils
{
	public static NumberFormatInfo GetNumberFormat(int nDecimalDigits)
	{
		NumberFormatInfo numberFormat = new CultureInfo("en-US", false).NumberFormat;
		numberFormat.NumberDecimalDigits = 0;
		return numberFormat;
	}
}
