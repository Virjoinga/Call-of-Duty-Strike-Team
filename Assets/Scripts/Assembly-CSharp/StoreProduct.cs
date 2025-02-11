public class StoreProduct
{
	public string productIdentifier;

	public string title;

	public string description;

	public string price;

	public string currencySymbol;

	public string formattedPrice;

	public float rawPrice;

	public override string ToString()
	{
		return string.Format("<StoreProduct>\nID: {0}\nTitle: {1}\nDescription: {2}\nPrice: {3}\nCurrency Symbol: {4}\nFormatted Price: {5}\nRaw Price {6}", productIdentifier, title, description, price, currencySymbol, formattedPrice, rawPrice);
	}
}
