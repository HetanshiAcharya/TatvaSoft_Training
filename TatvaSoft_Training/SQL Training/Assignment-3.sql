use hetanshiacharya_db
--Entering data into department table
insert into dbo.Department values
('101', 'HR'),
('102','IT'),
('103','Network'),
('104','Opertaions'),
('105','SW Support'),
('106','Design'),
('107','Planning')
select * from dbo.Department

--Entering data into employee table
insert into dbo.Employee values
('60','102','1000','Pranav','50000'),
('61','101','1050','Hetanshi','100000'),
('62','102','1060','Saurabh','120000'),
('63','106','1090','Mansi','35000'),
('64','103','1100','Dhruv','80000'),
('65','107','1420','Ram','52000'),
('66','107','1420','Nency','42000'),
('67','107','1420','Hari','25000'),
('68','107','1420','Om','14000')


select * from Employee

--Query 1--
--write a SQL query to find Employees who have the biggest salary in their Department--

select Department.dept_id, max(Employee.salary) as "Max salary", Department.dept_name
from Department left join Employee 
on Department.dept_id = Employee.dept_id
group by Department.dept_id, Department.dept_name
order by Department.dept_id

--Query 2--
--write a SQL query to find Departments that have less than 3 people in it--
select Department.dept_id, Department.dept_name, count(emp_id)as "no. of employee"
from Department left join Employee
on Department.dept_id = Employee.dept_id
group by Department.dept_id,Department.dept_name 
having count(emp_id)<= 3
order by Department.dept_id

--Query 3--
--write a SQL query to find All Department along with the number of people there--
select Department.dept_id, Department.dept_name, count(emp_id)as "no. of employee"
from Department left join Employee
on Department.dept_id = Employee.dept_id
group by Department.dept_id,Department.dept_name 
order by Department.dept_id

--Query 4--
--4. write a SQL query to find All Department along with the total salary there--
select Department.dept_id, Department.dept_name, sum(salary)as "Total Salary"
from Department left join Employee
on Department.dept_id = Employee.dept_id
group by Department.dept_id,Department.dept_name 
order by Department.dept_id