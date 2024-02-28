use hetanshiacharya_db
--adding values in salesman table --
insert into dbo.salesman (salesman_id,salesman_name,city, commission)
values
('300','Hetanshi','Ahmedabad','0.5'),
('301','Saurabh','Bharuch','0.7'),
('302','Riddhi','Bharuch','0.10'),
('303','Raj','Bopal','0.11'),
('304','Ankit','','0.10')
select * from salesman

--adding values in customer table--
insert into dbo.customer values 
('501','Nayana','Ahmedabad','A','300'),
('502','Meet','Bharuch','','305'),
('503','Bhoomi','Bopal','B','304'),
('505','Yashvi','Ahmedabad','B','303'),
('504','Aneri','Bharuch','B','301'),
('506','Riya','Somnath','B','300')
select * from dbo.customer

--adding values in order table--

insert into dbo.orderDemo values ('705','2500','2024-01-05','503','301'),
('706','896','2023-12-25','504','303'),
('707','500','2023-01-07','503','305'),
('708','700','2023-11-25','501','301'),
('710','1000','2024-01-03','500','306'),
('711','1000','2024-01-03','504','301')
select * from dbo.orderDemo
--Query 1--
--write a SQL query to find the salesperson and customer who reside in the same city.Return Salesman, cust_name and city
select salesman.salesman_name, customer.cust_name,salesman.city
from dbo.customer inner join dbo.salesman on dbo.customer.city=dbo.salesman.city 

--Query 2--
--write a SQL query to find those orders where the order amount exists between 500 and 2000. Return ord_no, purch_amt, cust_name, city
select orderDemo.ord_no,orderDemo.purch_amt, customer.cust_name, customer.city
from orderDemo inner join customer
on customer.customer_id = orderDemo.customer_id
where orderDemo.purch_amt between 500 and 2000

--Query 3--
--write a SQL query to find the salesperson(s) and the customer(s) he represents.Return Customer Name, city, Salesman, commission
select  salesman.salesman_name,customer.cust_name, salesman.city, salesman.commission
from dbo.customer inner join dbo.salesman on dbo.customer.salesman_id=dbo.salesman.salesman_id

--Query-4--
--write a SQL query to find salespeople who received commissions of more than 12 percent from the company. Return Customer Name, customer city, Salesman, 
--commission.
select customer.cust_name, customer.city, salesman.salesman_name, salesman.commission
from dbo.customer full outer join dbo.salesman on customer.salesman_id=salesman.salesman_id
where salesman.commission>0.12

--Query 5--
--write a SQL query to locate those salespeople who do not live in the same city where their customers live and have received a commission of more than 12% from the company. Return Customer Name, customer city, Salesman, salesman city,  commission
select customer.cust_name as "Customer Name", 
       customer.city, 
       salesman.salesman_name as "Salesman", 
       salesman.city, salesman.commission  
from customer  
INNER JOIN salesman   
on customer.salesman_id = salesman.salesman_id 
where salesman.commission > 0.12 
AND customer.city <> salesman.city;

--Query 6--
/* write a SQL query to find the details of an order. Return ord_no, ord_date, purch_amt, Customer Name, grade, Salesman, commission*/
select orderDemo.ord_date, orderDemo.ord_date, orderDemo.purch_amt, customer.cust_name as "Customer Name", customer.grade,salesman.city
from orderDemo inner join customer on orderDemo.customer_id = customer.customer_id
inner join salesman on salesman.salesman_id = orderDemo.salesman_id

--Query 7--
/*select *
from salesman  
join customer 
on customer.salesman_id = salesman.salesman_id
join orderDemo on customer.salesman_id= orderDemo.salesman_id 
SELECT * FROM dbo.orders NATURAL JOIN dbo.customer NATURAL JOIN dbo.salesman*/

--Query 8--
/*write a SQL query to display the customer name, customer city, grade, salesman, salesman city. The results should be sorted by ascending customer_id.*/
select customer.cust_name, customer.city as "Customer City", customer.grade,salesman.salesman_name, salesman.city as "Salesman City"
from customer left join salesman 
on customer.salesman_id = salesman.salesman_id
order by customer.customer_id

--Query-9--
/*write a SQL query to find those customers with a grade less than 300. Return cust_name, customer city, grade, Salesman, salesmancity. The result should be  ordered by ascending customer_id. */
select customer.customer_id, customer.city, customer.cust_name, customer.grade, salesman.salesman_name, salesman.city
from customer full join salesman on customer.salesman_id=salesman.salesman_id 
where customer.grade <> 'B'
order by customer.customer_id

--Query-10--
/*. Write a SQL statement to make a report with customer name, city, order number, order date, and order amount in ascending order according to the order date to  determine whether any of the existing customers have placed an order or not*/
select customer.cust_name,customer.customer_id, customer.city, orderDemo.ord_no, orderDemo.ord_date,orderDemo.purch_amt
from customer left join orderDemo  on orderDemo.customer_id = customer.customer_id
order by orderDemo.ord_date desc

--Query 11--
/*Write a SQL statement to generate a report with customer name, city, order number, order date, order amount, salesperson name, and commission to determine if any of the existing customers have not placed orders or if they have placed orders through  their salesman or by themselves*/
select customer.cust_name, customer.city, orderDemo.ord_no, orderDemo.ord_date,orderDemo.purch_amt, salesman.salesman_name, salesman.commission
from customer left join orderDemo 
on customer.customer_id= orderDemo.customer_id
left join salesman
on customer.salesman_id= salesman.salesman_id

--Query 12--
/*Write a SQL statement to generate a list in ascending order of salespersons who work either for one or more customers or have not yet joined any of the customers*/
select salesman.salesman_name, salesman.salesman_id,customer.cust_name 
from salesman left join customer
on salesman.salesman_id= customer.salesman_id
order by salesman.salesman_name

--Query 13--
/*. write a SQL query to list all salespersons along with customer name, city, grade, order number, date, and amount. */
select salesman.salesman_name, customer.cust_name,salesman.city,customer.grade, orderDemo.ord_no, orderDemo.ord_date,orderDemo.purch_amt 
from salesman left join customer
on salesman.salesman_id= customer.salesman_id
left join orderDemo
on customer.customer_id=orderDemo.customer_id

--Query-14--
/*Write a SQL statement to make a list for the salesmen who either work for one or  more customers or yet to join any of the customers. The customer may have placed,  either one or more orders on or above order amount 2000 and must have a grade, or   he may not have placed any order to the associated supplier*/
select salesman.salesman_name, customer.salesman_id
from salesman left join customer
on salesman.salesman_id = customer.salesman_id
join orderDemo
on orderDemo.customer_id= customer.customer_id
where orderDemo.purch_amt>=2000 and customer.grade is not null

--Query 15--
/* Write a SQL statement to generate a list of all the salesmen who either work for one or more customers or have yet to join any of them. The customer may have placed  one or more orders at or above order amount 2000, and must have a grade, or he may not have placed any orders to the associated supplier.*/
select salesman.salesman_name, customer.salesman_id
from salesman left join customer
on salesman.salesman_id = customer.salesman_id
join orderDemo
on orderDemo.customer_id= customer.customer_id
where orderDemo.purch_amt>=2000 and customer.grade is not null

--Query-16--
/*Write a SQL statement to generate a report with the customer name, city, order no. order date, purchase amount for only those customers on the list who must have a  grade and placed one or more orders or which order(s) have been placed by the  customer who neither is on the list nor has a grade*/
select customer.cust_name,customer.city,orderDemo.ord_date,orderDemo.purch_amt
from customer 
full join orderDemo 
on customer.customer_id= orderDemo.customer_id
where customer.grade IS NOT NULL;

--Query-17--
/*Write a SQL query to combine each row of the salesman table with each row of the customer table*/
select * from salesman cross join customer

--Query-18--
/*Write a SQL statement to create a Cartesian product between salesperson and  customer, i.e. each salesperson will appear for all customers and vice versa for that  salesperson who belongs to that city*/
select customer.cust_name, salesman.salesman_name  from customer  cross join salesman 
where salesman.city  is NOT NULL 

--Query 19--
/*Write a SQL statement to create a Cartesian product between salesperson and  customer, i.e. each salesperson will appear for every customer and vice versa for  those salesmen who belong to a city and customers who require a grade*/
select customer.cust_name, salesman.salesman_name  from customer  cross join salesman 
where salesman.city  is NOT NULL and customer.grade is NOT NULL

--Query 20--
/*Write a SQL statement to make a Cartesian product between salesman and  customer i.e. each salesman will appear for all customers and vice versa for those salesmen who must belong to a city which is not the same as his customer and the  customers should have their own grade */
select salesman.salesman_name,customer.cust_name
from salesman 
CROSS JOIN customer 
where salesman.city IS NOT NULL AND salesman.city != customer.city AND customer.grade IS NOT NULL;