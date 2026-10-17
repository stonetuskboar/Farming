using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeView : MonoBehaviour
{
    public TextMeshProUGUI SeasonText;
    public TextMeshProUGUI MonthText;
    public TextMeshProUGUI YearText;
    public Image BackImage;
    public RectTransform TimeRect;
    public Image CalendarImage;
    public Sprite SpringBack;
    public Sprite SummerBack;
    public Sprite FallBack;
    public Sprite WinterBack;
    public Sprite SpringCalendar;
    public Sprite SummerCalendar;
    public Sprite FallCalendar;
    public Sprite WinterCalendar;
    public void Set(Season season, int month, int year)
    {
        if(season == Season.春)
        {
            SeasonText.text = "春季";
            BackImage.sprite = SpringBack;
            CalendarImage.sprite = SpringCalendar;
        }
        else if (season == Season.夏)
        {
            SeasonText.text = "夏季";
            BackImage.sprite = SummerBack;
            CalendarImage.sprite = SummerCalendar;
        }
        else if (season == Season.秋)
        {
            SeasonText.text = "秋季";
            BackImage.sprite = FallBack;
            CalendarImage.sprite = FallCalendar;
        }
        else if (season == Season.冬)
        {
            SeasonText.text = "冬季";
            BackImage.sprite = WinterBack;
            CalendarImage.sprite = WinterCalendar;
        }
        MonthText.text = month.ToString();

        if(month % 3 == 1)
        {
            TimeRect.localRotation = Quaternion.Euler(new Vector3(0,0,-120));
        }
        else if(month % 3 == 2)
        {
            TimeRect.localRotation = Quaternion.Euler(new Vector3(0, 0, -60));
        }
        else
        {
            TimeRect.localRotation = Quaternion.Euler(new Vector3(0, 0, -12));
        }
        if(year == 1)
        {
            YearText.text = "一";
        }
        else if(year == 2)
        {
            YearText.text = "二";
        }
        else if (year == 3)
        {
            YearText.text = "三";
        }
        else if (year == 4)
        {
            YearText.text = "四";
        }
        else if (year == 5)
        {
            YearText.text = "五";
        }
    }
}
