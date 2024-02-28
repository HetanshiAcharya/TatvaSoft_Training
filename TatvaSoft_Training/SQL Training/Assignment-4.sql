--Query 1--
--Create a stored procedure in the Northwind database that will calculate the average 
--value of Freight for a specified customer.Then, a business rule will be added that will 
--be triggered before every Update and Insert command in the Orders controller,and 
--will use the stored procedure to verify that the Freight does not exceed the average 
--freight. If it does, a message will be displayed and the command will be cancelled.create procedure insertUpdateCust @freight int
as
DECLARE @Avg_freight AS int
SET @Avg_freight = avg(Freight)

IF @freight>@Avg_freight
begin
insert into Orders values ('500','	MKJI','678','2024-02-25','2024-30-25','2024-02-01','','520000','','','','','','')
end

exec insertCustomerDetails  @orderId='1',@productId='2',@unitPrice='300.2',@quantity='4',@discount='500.0'

--Query2--
--write a SQL query to Create Stored procedure in the Northwind database to retrieve Employee Sales by Country--
create procedure RetrieveDataSalesbyCountry @country varchar(30) 
as
select Salesperson, Country, sum(UnitPrice * Quantity) as "Total Price"
from dbo.Invoices
where Country = @country
group by Salesperson, Country

exec RetrieveDataSalesbyCountry @country =Brazil

--Query3--
--write a SQL query to Create Stored procedure in the Northwind database to retrieve Sales by Year--
create procedure RetrieveDataSalesbyYear @year int 
as
select OrderDate, sum(UnitPrice * Quantity) as "Total Price"
from dbo.Invoices
where Year(OrderDate) = @year
group by OrderDate
exec RetrieveDataSalesbyYear @year ='1997'

--Query 4--
--write a SQL query to Create Stored procedure in the Northwind database to retrieve Sales By Category--

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SalesByCategory]
    @CategoryName nvarchar(15), @OrdYear nvarchar(4) = '1998'
AS
IF @OrdYear != '1996' AND @OrdYear != '1997' AND @OrdYear != '1998' 
BEGIN
	SELECT @OrdYear = '1998'
END

SELECT ProductName,
	TotalPurchase=ROUND(SUM(CONVERT(decimal(14,2), OD.Quantity * (1-OD.Discount) * OD.UnitPrice)), 0)
FROM [Order Details] OD, Orders O, Products P, Categories C
WHERE OD.OrderID = O.OrderID 
	AND OD.ProductID = P.ProductID 
	AND P.CategoryID = C.CategoryID
	AND C.CategoryName = @CategoryName
	AND SUBSTRING(CONVERT(nvarchar(22), O.OrderDate, 111), 1, 4) = @OrdYear
GROUP BY ProductName
ORDER BY ProductName

exec SalesByCategory @CategoryName ='Beverages'


--Query 5-- 
--write a SQL query to Create Stored procedure in the Northwind database to retrieve Ten Most Expensive Products
create  procedure TenMostExpensiveProducts as
select top 10 ProductName, (UnitPrice*Quantity) as "sales"  from dbo.Invoices order by UnitPrice*Quantity

exec TenMostExpensiveProducts 

--Query 6--
--write a SQL query to Create Stored procedure in the Northwind database to insert Customer Order Details 
create procedure insertCustomerDetails @orderId int, @productId int, @unitPrice int, @quantity int, @discount int
as
insert into [dbo].[Order Details] ([dbo].[Order Details].OrderID, [dbo].[Order Details].ProductID,[dbo].[Order Details].UnitPrice,[dbo].[Order Details].Quantity, [dbo].[Order Details].Discount) 
values (@orderId, @productId, @unitPrice, @quantity, @discount)

exec insertCustomerDetails  @orderId='1',@productId='2',@unitPrice='300.2',@quantity='4',@discount='500.0'

--Query 7 --
--write a SQL query to Create Stored procedure in the Northwind database to update Customer Order Details
alter procedure insertCustomerDetails @orderId int, @productId int, @unitPrice money, @quantity int, @discount float
as
update [dbo].[Order Details]
set [dbo].[Order Details].OrderID=@orderId,[dbo].[Order Details].ProductID=@productId,[dbo].[Order Details].UnitPrice=@unitPrice,[dbo].[Order Details].Quantity=@quantity,[dbo].[Order Details].Discount=@discount
where OrderID=@orderId


exec insertCustomerDetails  @orderId='10248',@productId='2',@unitPrice='30.0',@quantity='4',@discount='0.2'
