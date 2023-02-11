namespace ITMO.Dev.ASAP.DataImport.Models;

public readonly record struct GroupName(string Name)
{
    public static GroupName FromShortName(int value)
    {
        return new GroupName($"M32{value:00}1");
    }
}