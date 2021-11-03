using UnityEngine;
using System;
using System.Collections.Generic;

public class UtilityMethods
{
    public const int TIMEOUT = -99;
    public static bool isAllowEnterClass(string dayOfWeek, int startTime, int allowTime)
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

    public static List<string> ListUpInvitingStudents(object classObject)
    {
        ClassData classData = JsonUtility.FromJson<ClassData>(classObject.ToString());

        List<string> studentIds = new List<string>();

        for (int count = 0; count < classData.students.Count; count++)
        {
            studentIds.Add(classData.students[count].studentId);
        }

        return studentIds;
    }
}