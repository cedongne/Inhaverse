using UnityEngine;
using System;
using System.Collections.Generic;

public class UtilityMethods
{
    // Open Semester
    public const int year = 2021;
    public const int month = 8;
    public const int day = 30;

    public const int TIMEOUT = -99;

    public static string[] SplitTimeTableUserData(string timeTableData)
    {
        // 0 : Class number
        // 1 : Class id
        // 2 : Class instructor
        // 3 : First class day of week
        // 4 : First class start time
        // 5 : First class end time
        // 6 : Second class Day of week
        // 7 : Second class start time
        // 8 : Second class end time
        char[] delimiters = { ',', '~' };
        string[] splitedTimeTableString = timeTableData.Split(delimiters);

        return splitedTimeTableString;
    }

    public static bool DetermineIsClassTime(string dayOfWeek, int startTime, int endTime)
    {
        int nowClassTimeHour = ((DateTime.Now.Hour - 9) * 2);
        int nowClassTimeMinute = 0;
        int nowClassTime;

        if (DateTime.Now.Minute >= 45)
            nowClassTimeMinute = 3;
        else if (DateTime.Now.Minute < 15)
            nowClassTimeMinute = 1;
        else if (DateTime.Now.Minute >= 15 && DateTime.Now.Minute < 45)
            nowClassTimeMinute = 2;

        nowClassTime = nowClassTimeHour + nowClassTimeMinute;
        Debug.Log("NowDayOfWeek : " + DateTime.Now.DayOfWeek + ", ClassDayOfWeek : " + dayOfWeek);
        Debug.Log("NowClassTime : " + (nowClassTime) + ", ClassTime : " + startTime + " to " + endTime);

        if (dayOfWeek.Equals(DateTime.Now.DayOfWeek.ToString()))
            if (nowClassTime >= startTime && nowClassTime <= endTime)
                return true;
            else
                return false;
        else
            return false;
    }

    public static bool DetermineAllowClassEnter(string[] splitedTimeTableString)
    {
        bool isAllowEnterClass;

        isAllowEnterClass = DetermineIsClassTime(splitedTimeTableString[3], int.Parse(splitedTimeTableString[4]), int.Parse(splitedTimeTableString[5]));
        if (!isAllowEnterClass && splitedTimeTableString.Length > 3)
            isAllowEnterClass = DetermineIsClassTime(splitedTimeTableString[6], int.Parse(splitedTimeTableString[7]), int.Parse(splitedTimeTableString[8]));

        return isAllowEnterClass;
    }

    public static List<string> ListUpInvitingStudents(object classObject)
    {
        ClassData classData = JsonUtility.FromJson<ClassData>(JsonUtility.ToJson(classObject));

        List<string> studentIds = new List<string>();

        for (int count = 0; count < classData.students.Count; count++)
        {
            studentIds.Add(classData.students[count].schoolId);
        }

        return studentIds;
    }



    public static string ConvertToDayOfWeek(int dayCode)
    {
        if (dayCode == 0)
            return "Monday";
        else if (dayCode == 1)
            return "Tuesday";
        else if (dayCode == 2)
            return "Wednesday";
        else if (dayCode == 3)
            return "Thursday";
        else if (dayCode == 4)
            return "Friday";
        else
            return "";
    }

    public static int ConvertToDayCode(string dayOfWeek)
    {
        if (dayOfWeek == "Monday")
            return 0;
        else if (dayOfWeek == "Tuesday")
            return 1;
        else if (dayOfWeek == "Wednesday")
            return 2;
        else if (dayOfWeek == "Thursday")
            return 3;
        else if (dayOfWeek == "Friday")
            return 4;
        else
            return 0;

    }

    public static string ConvertDayOfWeekToKorean(string dayOfWeek)
    {
        if (dayOfWeek == "Monday")
            return "월";
        else if (dayOfWeek == "Tuesday")
            return "화";
        else if (dayOfWeek == "Wednesday")
            return "수";
        else if (dayOfWeek == "Thursday")
            return "목";
        else if (dayOfWeek == "Friday")
            return "금";
        else
            return "주말";
    }

    public static int GetWeekOfSemester()
    {
        int elapsedDays = Convert.ToInt32((DateTime.Now - new DateTime(year, month, day)).TotalDays) - 1;

        int calWeek = (elapsedDays / 7) + 1;

        return calWeek;
    }

    public static int Exponential(int _base, int _exponent)
    {
        int result = 1;

        for(int count = 0; count < _exponent; count++)
        {
            result *= _base;
        }
        return result;
    }

    public static string ReverseString(string originString)
    {
        char[] originChar = originString.ToCharArray();
        char tmp;
        for(int count = 0; count< originString.Length / 2; count++)
        {
            tmp = originChar[count];
            originChar[count] = originChar[originChar.Length - (count + 1)];
            originChar[originChar.Length - (count + 1)] = tmp;
        }
        string resultString = new string(originChar);
        Debug.Log(resultString);

        return resultString;
    }
}