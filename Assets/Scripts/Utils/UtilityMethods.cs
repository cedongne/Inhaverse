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
        // 0 : Class ID
        // 1 : Class instructor
        // 2 : First class day of week
        // 3 : First class start time
        // 4 : First class end time
        // 5 : Second class Day of week
        // 6 : Second class start time
        // 7 : Second class end time
        // 8 : Late allow time
        char[] delimiters = { ',', '~' };
        string[] splitedTimeTableString = timeTableData.Split(delimiters);

        return splitedTimeTableString;
    }

    public static bool DetermineIsClassTime(string dayOfWeek, int startTime, int allowTime)
    {
        int nowClassTimeHour = (DateTime.Now.Hour - 8) * 2;
        int nowClassTimeMinute = 0;

        if ((DateTime.Now.Minute >= 45 && DateTime.Now.Minute <= 59) || DateTime.Now.Minute >= 0 && DateTime.Now.Minute <= allowTime)
            nowClassTimeMinute = 3;
        else if (DateTime.Now.Minute >= 15 && DateTime.Now.Minute <= 30 + allowTime)
            nowClassTimeMinute = 2;
        else
            nowClassTimeMinute = TIMEOUT;

        int nowClassTime = nowClassTimeHour + nowClassTimeMinute;

        Debug.Log(DateTime.Now.DayOfWeek.ToString() + " " + dayOfWeek);
        Debug.Log(startTime + " " + nowClassTime);
        if (startTime == nowClassTime && dayOfWeek == DateTime.Now.DayOfWeek.ToString())
            return true;
        else
            return false;
    }

    public static bool DetermineAllowClassEnter(string[] splitedTimeTableString)
    {
        bool isAllowEnterClass;

        isAllowEnterClass = DetermineIsClassTime(splitedTimeTableString[2], int.Parse(splitedTimeTableString[3]), 10);
        if (!isAllowEnterClass && splitedTimeTableString.Length > 3)
            isAllowEnterClass = DetermineIsClassTime(splitedTimeTableString[5], int.Parse(splitedTimeTableString[6]), 10);

        return isAllowEnterClass;
    }

    public static List<string> ListUpInvitingStudents(object classObject)
    {
        ClassData classData = JsonUtility.FromJson<ClassData>(JsonUtility.ToJson(classObject));

        List<string> studentIds = new List<string>();

        for (int count = 0; count < classData.students.Count; count++)
        {
            studentIds.Add(classData.students[count].studentId);
        }

        return studentIds;
    }

    public static string ConvertToDayOfWeek(int dayCode)
    {
        if (dayCode == 0)
            return "Monday";
        else if (dayCode == 1)
            return "TuesDay";
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
            return "";
    }

    public static int GetWeekOfSemester()
    {
        int elapsedDays = Convert.ToInt32((DateTime.Now - new DateTime(year, month, day)).TotalDays) - 1;

        int calWeek = (elapsedDays / 7) + 1;

        return calWeek;
    }
}