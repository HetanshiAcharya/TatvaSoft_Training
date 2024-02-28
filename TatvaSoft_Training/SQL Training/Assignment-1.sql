SELECT ProductID, ProductName, UnitPrice FROM hetanshiacharya_db.dbo.Products where UnitPrice<20;

SELECT ProductID, ProductName, UnitPrice FROM hetanshiacharya_db.dbo.Products where UnitPrice BETWEEN 15 AND 25;

select avg(UnitPrice) from hetanshiacharya_db.dbo.Products;
SELECT ProductName, UnitPrice from  hetanshiacharya_db.dbo.Products where UnitPrice > (select avg(UnitPrice) from hetanshiacharya_db.dbo.Products)
select top 10 ProductName, UnitPrice from hetanshiacharya_db.dbo.Products order by UnitPrice desc;
select * from hetanshiacharya_db.dbo.Products
select ProductName, UnitsInStock, UnitsOnOrder from hetanshiacharya_db.dbo.Products where UnitsInStock < UnitsOnOrder