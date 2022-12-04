using UnitOfWorkPatternWithDapper;

using (var unitOfWork = new UnitOfWork(_connectionString)) 
{ 
    var newEmployee = new Employee 
    { 
        FirstName = "Ian", 
        LastName = "Rufus", 
        JobTitle = "Developer" 
    };
    newEmployee.EmployeeId = unitOfWork.EmployeeRepository.Add(newEmployee); 
    newEmployee.JobTitle = "Fired!"; 
    unitOfWork.EmployeeRepository.Update(newEmployee);
    unitOfWork.Commit();
}