[System.Serializable]
public class CoinConfig
{
    public float rotationSpeed;
    public float coinScore;
    public float growthRate;
}

[System.Serializable]
public class HeartConfig
{
    public int healAmount;
}

[System.Serializable]
public class ItemConfig
{
    public CoinConfig Coin;
    public HeartConfig Heart;
}