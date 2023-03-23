namespace TravEx.Model;

public record Listing(string Ware, int? BuyPrice, double? BuyPriceFactor, int SellPrice, double SellPriceFactor, int? AvailableTonnage);