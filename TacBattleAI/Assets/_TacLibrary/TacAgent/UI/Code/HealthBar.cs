using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(FollowCameraRotation))]
public class HealthBar : MonoBehaviour
{
	public Image Image;
	public Text Text;

	public float AnimationSpeed = 1.0f;
	public bool ShowText = false;
	public bool ShowPercentage = false;
    public MaxValue CurrentHealth;

	float finalValue;
    float leftoverAmount = 0f;


    Coroutine currentChangeHealth;

    void Update()
    {
        if (Text != null && ShowText == true)
        {
            if (ShowPercentage)
            {
                Text.text = CurrentHealth.Percentage.ToString("F0") + " %";
            }
            else
            {
                Text.text = CurrentHealth.Current.ToString("F0") + " / " + CurrentHealth.Maximum.ToString("F0");
            }
        }
    }

    public void ChangeHealth(MaxValue argCurrentHealth)
    {
        CurrentHealth = argCurrentHealth;

		if (currentChangeHealth != null)
        { 
            StopCoroutine(currentChangeHealth);
        }
		currentChangeHealth = StartCoroutine(Change(argCurrentHealth));
    }

    private IEnumerator Change(MaxValue argCurrentHealth)
    {
        finalValue = argCurrentHealth.Percentage / 100;

        float cacheLeftoverAmount = leftoverAmount;

        float timeElapsed = 0;

        while (timeElapsed < AnimationSpeed)
        {
            float leftover = Mathf.Lerp((argCurrentHealth.Previous / argCurrentHealth.Maximum) + cacheLeftoverAmount, finalValue, timeElapsed / AnimationSpeed);
            leftoverAmount = leftover - finalValue;
            Image.fillAmount = leftover;
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        leftoverAmount = 0;
        Image.fillAmount = finalValue;
    }
}

[System.Serializable]
public class MaxValue
{
	public float Current;
	public float Previous;
	public float Maximum;

	public float Percentage
    {
        get { return Current * 100 / Maximum; }
    }

	public MaxValue(float argCurrentValue, float argPreviousValue, float argMaximumValue)
	{
        Current = argCurrentValue;
		Previous = argPreviousValue;
		Maximum = argMaximumValue;
	}
}
