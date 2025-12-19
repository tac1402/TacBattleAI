[System.Serializable]
public class LimitCircle
{
	public Vector2_ Center;
	public float Radius;

	public LimitCircle() { }
	public LimitCircle(Vector2_ argCenter, float argRadius)
	{
		Center = argCenter;
		Radius = argRadius;
	}
}
