using System;

public class StoreTransaction
{
	public string productIdentifier;

	public string base64EncodedTransactionReceipt;

	public int quantity;

	public static StoreTransaction transactionFromString(string transactionString)
	{
		StoreTransaction storeTransaction = new StoreTransaction();
		string[] array = transactionString.Split(new string[1] { "|||" }, StringSplitOptions.None);
		if (array.Length == 3)
		{
			storeTransaction.productIdentifier = array[0];
			storeTransaction.base64EncodedTransactionReceipt = array[1];
			storeTransaction.quantity = int.Parse(array[2]);
		}
		return storeTransaction;
	}

	public override string ToString()
	{
		return string.Format("<StoreTransaction>\nID: {0}\nReceipt: {1}\nQuantity: {2}", productIdentifier, base64EncodedTransactionReceipt, quantity);
	}
}
