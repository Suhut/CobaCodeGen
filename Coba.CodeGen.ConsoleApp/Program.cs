using Coba.CodeGen.ConsoleApp.Model; 

Console.WriteLine("---------------------------------------");
Console.WriteLine("  Wired Brain Coffee - Employee Manager  ");
Console.WriteLine("---------------------------------------");
Console.WriteLine();

var person = new Employee
{
    FirstName = "Thomas",
    LastName = "Huber"
};

var personAsString = person.ToString();

Console.WriteLine(personAsString);

Console.ReadLine();