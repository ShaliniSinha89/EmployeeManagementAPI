﻿using EmployeeManagement.DbCon;
using EmployeeManagement.DbModel;
using EmployeeManagement.Helper;
using EmployeeManagement.Model;
using EmployeeManagement.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Eventing.Reader;

namespace EmployeeManagement.Services
{
    public class EmployeeMgntServices : IEmployeeMgnt
    {
        private readonly EmployeeContext _employeecontext;

        public EmployeeMgntServices(EmployeeContext employeecontext)
        {
            _employeecontext = employeecontext;
        }
        public async Task<ApiResponse> AddNewParentDepartment(ParentModel parent)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                var parentdata = _employeecontext.ParentDepartments.Where(x => x.DepartmentName == parent.DepartmentName).FirstOrDefault();
                if (parentdata == null)
                {
                    var parentdept = new ParentDepartment()
                    {
                        DepartmentName = parent.DepartmentName,
                        DepartmentLogo = parent.DepartmentLogo

                    };
                    await _employeecontext.ParentDepartments.AddAsync(parentdept);
                    if (await _employeecontext.SaveChangesAsync() > 0)
                    {
                        apiResponse.Code = 200;
                        apiResponse.Result = "true";
                    }
                    else
                    {
                        apiResponse.Code = 400;
                        apiResponse.Result = "false";
                    }
                }
                else
                {
                    apiResponse.Code = 409;
                    apiResponse.Result = "Already Exists";
                }
            }
            catch (Exception)
            {

                throw;
            }
            return apiResponse;
        }

        public async Task<ApiResponse> AddChildDepartment(ChildModel child)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                var childdata = _employeecontext.ChildDepartments.Where(x => x.ParentDepartmentId == child.ParentDepartmentId && x.DepartmentName ==
                child.DepartmentName).FirstOrDefault();
                if (childdata == null)
                {
                    var childdept = new ChildDepartment()
                    {
                        ParentDepartmentId = child.ParentDepartmentId,
                        DepartmentName = child.DepartmentName
                    };
                    await _employeecontext.ChildDepartments.AddAsync(childdept);
                    if (await _employeecontext.SaveChangesAsync() > 0)
                    {
                        apiResponse.Code = 200;
                        apiResponse.Result = "true";
                    }
                    else
                    {
                        apiResponse.Code = 400;
                        apiResponse.Result = "BadRequest!";
                    }

                }
                else
                {
                    apiResponse.Code = 409;
                    apiResponse.Result = "already exist";
                }
            }
            catch (Exception)
            {

                throw;
            }
            return apiResponse;
        }
        public async Task<List<ParentModel>> GetParentDepartment()
        {
            List<ParentModel> parent = new List<ParentModel>();

            try
            {
                var parentsData = _employeecontext.ParentDepartments.ToList();
                //if(parentsData.Count()>0)
                //{
                //    foreach (var item in parentsData)
                //    {
                //        ParentModel ptmodel = new ParentModel();
                //        {
                //            ptmodel.DepartmentId = item.DepartmentId;
                //            ptmodel.DepartmentName = item.DepartmentName;
                //            ptmodel.DepartmentLogo = item.DepartmentLogo;
                //        }
                //        parent.Add(ptmodel);
                //    }
                //    return parent;
                //}

                // Other way to find the data in list : 
                parent = (from item in parentsData
                          where parentsData.Count() > 0
                          select new ParentModel()
                          {
                              DepartmentId = item.DepartmentId,
                              DepartmentName = item.DepartmentName,
                              DepartmentLogo = item.DepartmentLogo
                          }).ToList();

                return parent;
            }
            catch (Exception ex)
            {

                throw new Exception("An error occured while retrieving user.", ex);
            }

            return parent;

        }
        public async Task<List<ChildModel>> GetAllChildDepartment()
        {
            List<ChildModel> children = new List<ChildModel>();
            try
            {
                var data = await _employeecontext.ChildDepartments.ToListAsync();
                if (data.Count() > 0)
                {
                    foreach (var item in data)
                    {
                        ChildModel model = new ChildModel();
                        {
                            model.ChildDepartmentId = item.ChildDepartmentId;
                            model.ParentDepartmentId = item.ParentDepartmentId;
                            model.DepartmentName = item.DepartmentName;
                        }
                        children.Add(model);
                    }
                    return children;
                }
            }
            catch (Exception ex)
            {

                throw new Exception("An error occured while retrieving user.", ex);
            }

            return children;

        }

        public async Task<List<ChildDepartment>> GetChildDepartmentByParentId(int departmentId)
        {
            List<ChildDepartment> child = new List<ChildDepartment>();
            try
            {
                var childData = await _employeecontext.ChildDepartments.Where(x => x.ParentDepartmentId == departmentId).ToListAsync();
                if (childData.Count() > 0)
                {
                    child = (from item in childData
                             select new ChildDepartment()
                             {
                                 ChildDepartmentId = item.ChildDepartmentId,
                                 ParentDepartmentId = item.ParentDepartmentId,
                                 DepartmentName = item.DepartmentName
                             }).ToList();
                    return child;
                }
                else
                {
                    return new List<ChildDepartment>();
                }
            }
            catch (Exception)
            {
                return new List<ChildDepartment>();
            }
        }

        public async Task<List<EmployeeModel>> GetAllEmployee()
        {
            List<EmployeeModel> emp = new List<EmployeeModel>();
            try
            {
                var data = _employeecontext.Employees.ToList();
                if (data.Count() > 0)
                {
                    foreach (var item in data)
                    {
                        EmployeeModel model = new EmployeeModel();
                        {
                            model.EmployeeId = item.EmployeeId;
                            model.EmployeeName = item.EmployeeName;
                            model.EmailId = item.EmailId;
                            model.DepartmentId = item.DepartmentId;
                            model.ContactNo = item.ContactNo;
                            model.Gender = item.Gender;
                            model.Password = item.Password;
                            model.Role = item.Role;

                        }
                        emp.Add(model);
                    }
                    return emp;
                }
                return new List<EmployeeModel>();
            }
            catch (Exception ex)
            {
                return new List<EmployeeModel>();
            }
        }

        public async Task<EmployeeModel> GetEmployeeById(int id)
        {
            EmployeeModel emp = new EmployeeModel();
            try
            {
                var userdata = _employeecontext.Employees.Where(x => x.EmployeeId == id).FirstOrDefault();
                if (userdata == null)
                {
                    return new EmployeeModel();
                }
                else
                {
                    emp.EmployeeName = userdata.EmployeeName;
                    emp.ContactNo = userdata.ContactNo;
                    emp.Role = userdata.Role;
                    emp.Gender = userdata.Gender;
                    emp.Password = userdata.Password;
                    emp.DepartmentId = userdata.DepartmentId;
                    emp.EmailId = userdata.EmailId;
                    emp.EmployeeId = userdata.EmployeeId;

                }
            }
            catch (Exception)
            {

                throw;
            }
            return emp;
        }

        public async Task<ApiResponse> AddEmployee(EmployeeModel employee)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {

                var empdata = _employeecontext.Employees.Where(x => x.EmailId == employee.EmailId && x.ContactNo == employee.ContactNo).FirstOrDefault();
                if (empdata == null)
                {
                    var empuser = new Employee()
                    {
                        EmployeeName = employee.EmployeeName,
                        EmailId = employee.EmailId,
                        ContactNo = employee.ContactNo,
                        Role = employee.Role,
                        Password = employee.Password,
                        Gender = employee.Gender,
                        DepartmentId = employee.DepartmentId,

                    };
                    await _employeecontext.Employees.AddAsync(empuser);
                    if (await _employeecontext.SaveChangesAsync() > 0)
                    {
                        apiResponse.Code = 200;
                        apiResponse.Message = "Data Added Successfully!";
                    }
                    else
                    {
                        apiResponse.Code = 400;
                        apiResponse.Message = "Bad Request";
                    }
                }
                else
                {
                    apiResponse.Code = 409;
                    apiResponse.Message = "User already exist with same EmailId and ContactNo";
                }

            }
            catch (Exception)
            {

                throw;
            }
            return apiResponse;
        }

        public async Task<ApiResponse> UpdateEmployee(EmployeeModel employee)
        {
            ApiResponse apiResponse = new ApiResponse();

            try
            {
                var empdata = _employeecontext.Employees.Where(x => x.EmployeeId == employee.EmployeeId).FirstOrDefault();
                if (empdata == null)
                {
                    apiResponse.Code = 404;
                    apiResponse.Message = "Employee Not Found";
                    return apiResponse;
                }
                empdata.EmployeeName = employee.EmployeeName;
                empdata.ContactNo = employee.ContactNo;
                empdata.Password = employee.Password;
                empdata.DepartmentId = employee.DepartmentId;
                empdata.EmailId = employee.EmailId;
                empdata.Gender = employee.Gender;
                empdata.Role = employee.Role;
                _employeecontext.Employees.Update(empdata);
                if (await _employeecontext.SaveChangesAsync() > 0)
                {
                    apiResponse.Code = 200;
                    apiResponse.Message = "Data Updated Successfully";
                    return apiResponse;
                }

            }
            catch (Exception ex)
            {
                apiResponse.Code = 500;
                apiResponse.Message = "ex.Message";
                return apiResponse;
            }
            return apiResponse;
        }

        public async Task<ApiResponse> DeleteEmployee(int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                var data = _employeecontext.Employees.Where(x => x.EmployeeId == id).FirstOrDefault();
                if (data == null)
                {
                    apiResponse.Code = 404;
                    apiResponse.Message = "user not found!";
                    return apiResponse;
                }
                _employeecontext.Employees.Remove(data);

                apiResponse.Code = 200;
                apiResponse.Message = "user deleted successfully!";
                return apiResponse;

            }
            catch (Exception ex)
            {

                apiResponse.Code = 500;
                apiResponse.Message = ex.Message;
                return apiResponse;
            }
            return apiResponse;
        }


    }
}
