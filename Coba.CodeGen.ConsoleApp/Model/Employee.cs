
using Coba.CodeGen.Generators;

namespace Coba.CodeGen.ConsoleApp.Model;

[GenerateToString]
public partial class Employee
{
    public string? FirstName { get; set; }
    internal string? MidleName { get; set; } 
    public string? LastName { get; set; } 
}
