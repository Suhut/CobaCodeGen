using Coba.CodeGen.ConsoleApp.Model;  

var employee = new Employee
{
    FirstName = "Suhut",
    MidleName = "Wadiyo",
    LastName = "Padang"
};

var employeeAsString = employee.ToString();

Console.WriteLine(employeeAsString); 

Console.ReadLine();