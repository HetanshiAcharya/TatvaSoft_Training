/*FOLDER 4 EXERCISE-1*/
-- The "payment" table stores customer ids and amount they have paid. I want to get the list of customer ids that have rented out movies that cost 7.99 or more so I can put them in a "Platinum" category and send them coupons later.
-- Can you give me a report containing that?

SELECT DISTINCT customer_id
FROM payment
WHERE amount>=7.99;

/*FOLDER 4 EXERCISE-2*/
-- The "film" table contains details of films we rent, including their name, rental cost and replacement cost. Take a look at the table and generate a query that tells me high value films, meaning those that:
-- a: have rental rate more than 2.99, OR
-- b: replacement cost more than 19.99
-- I want the report to be very simple, so just include the movie name, rental cost and replacement cost in there.

SELECT title AS "Movie", rental_rate AS "Rent", replacement_cost AS "Replacement"
FROM film
WHERE rental_rate>2.99 OR replacement_cost>19.99;

/*FOLDER 6 EXERCISE-1*/
-- The "film" table has the replacement cost and rental duration (assume, in weeks) for each movie.
-- Can you tell me the movies (just the title and relevant details) sorted by highest to lowest replacement cost and rented out for 4-6 weeks.
-- Also I don't want all the data, just give me 100 records. 

SELECT title, replacement_cost, rental duration
FROM film
ORDER BY replacement_cost DESC;

/*FOLDER 6 EXERCISE-2*/
-- I want a list of all movies that either have the rating 'G' or 'PG'.
-- The movies must be longer than 120 minutes in length, and they should have the word 'Action' anywhere in the description.

SELECT title, rating, length, description
FROM film
WHERE rating IN ('G' , 'PG')
AND length > 120
AND description LIKE '%Actions';


/*FOLDER 8 EXERCISE-1*/
--The "actor" table has names of all actors. Can you tell me which actor first names are most common and how many records have it?

SELECT DISTINCT first_name, COUNT(*)
FROM actor
GROUP BY first_name 
ORDER BY count DESC;
select * from rental
select * from customer

/*FOLDER 8 EXERCISE-2*/
-- I want something to analyse the pattern between film language and its rental rate. Send me a data extract (a report) that shows language of each movie, its title and the rental rate.

SELECT f.title, f.rental_rate, l.name 
FROM film f
JOIN language l
ON f.language_id =l.language_id
ORDER BY rental_rate DESC;

/*FOLDER 10 EXERCISE-1*/
-- Show me a list of actors and the count of movies they have acted in. Sort it in a way that the actor who has done most movies is at top.

SELECT actor.actor_id, CONCAT (actor.first_name,' ',actor.last_name) AS "Actor Name",
count(film.film_id) AS "Movies Acted In"
FROM actor
JOIN film_actor
ON actor.actor_id = film_actor.actor_id
JOIN film
ON film.film_id=film_actor.film_id
GROUP BY actor.actor_id
ORDER BY "Movies Acted In" DESC;

/*FOLDER 10 EXERCISE-2*/
-- I want to create a report, that shows the different ratings of all movies, and count of movies that have been rented for each rating

SELECT film.rating, COUNT inventory.inventory_id) 
FROM rental
JOIN inventory
ON rental.inventory_id=inventory.inventory_id 
RIGHT JOIN FILM
ON inventory. film_id =film.film_id
GROUP BY film.rating
ORDER BY COUNT (inventory.inventory_id) DESC;

/*FOLDER 12 EXERCISE-1*/
/* return rental date, ruteun date, customer first name, kast name and email whose rental duration is more than 7 days*/
select rental.rental_date, rental.return_date, AGE(rental_date, return_date) as "Rental Duration",
customer.first_name, customer.last_name, customer.email
from rental join customer
on rental.customer_id = customer.customer_id
where "Rental Duration">=7

/*FOLDER 12 EXERCISE-2*/
/*return string after the 10th character
return string after the 15th character
return string after 5th character just 3 word
return string after 3th character just 1 word*/

select title, substr(title,10) from film
select title, substr(title,15) from film
select title,length(title), substr(title,15) from film
select title,length(title), substr(title,5,3) from film
select title,length(title), substr(title,5,1) from film



/*EXERCISE-1 FOLDER 14*/
/*report name as firstname + lastname, email, total rental and other column as customer category 
if total amount>=200 then elite,200>sum(amount)>=150 then 'Platinum' sum(amount)>=100 then 'Gold' sum(amount)>=0 then 'Silver'*/
select concat(first_name, ' ', last_name) as "Customer Name", email, sum(amount) as "Total Rentals",
case
	when sum(amount)>=200 then 'Elite'
	when sum(amount)>=150 then 'Platinum'
	when sum(amount)>=100 then 'Gold'
	when sum(amount)>=0 then 'Silver'
end as "Customer Category"
from customer left join payment 
on customer.customer_id=payment.customer_id
group by "Customer Name",email



/*EXERCISE-2 FOLDER 12*/
/*create view for above execise*/

create view payment_category as
select concat(first_name, ' ', last_name) as "Customer Name", email, sum(amount) as "Total Rentals",
case
	when sum(amount)>=200 then 'Elite'
	when sum(amount)>=150 then 'Platinum'
	when sum(amount)>=100 then 'Gold'
	when sum(amount)>=0 then 'Silver'
end as "Customer Category"
from customer left join payment 
on customer.customer_id=payment.customer_id
group by "Customer Name",email
select * from payment_category


Coding Exercise 1

You are working in a fictional Ecommerce company called MyCommerce. We want to setup a new database and an order details table inside it. Please execute the following steps:
1: Create a database called "mycommerce"
2: Create a table inside it called "order_details", that has following fields
	a: orderid - all our purchases have an order#, which is unique and numeric
	b: customer_name
	c: product_name -> text values
	d: ordered_from-> where the customer place order from (store/app/website)
	e: order amount -> do not lose on decimals!!
	f: order_date -> when the order was placed
	g: delivery_date -> when the order was delivered
3: Insert 50 records in the table. Use the attached notepad in the "Resources" section to save you time.
4: Once all setup is done, run a query to return me product, number of orders, total sales

CREATE DATABASE mycommerce;

CREATE TABLE order_details(
	orderid INTEGER PRIMARY KEY,
	customer_name VARCHAR(50) NOT NULL,
	product_name VARCHAR(50) NOT NULL,
	ordered_from VARCHAR(50) NOT NULL,
	order_amount NUMERIC(7,2),
	order date DATE NOT NULL,
	delivery_date DATE
)

(1001, 'Linda', 'Pen', 'Store', 9.82, '2025-01-02','2025-01-06'),
(1002, 'Stephanie', 'Pencil', 'App', 12.79, '2025-01-04','2025-01-08'),
(1003, 'Deborah', 'Scissors', 'Website', 13.68, '2025-01-07', '2025-01-12'),
(1004, 'Andrew', 'Pen', 'Website', 7.62, '2025-01-07', '2025-01-15'),
(1005, 'Steven', 'Chair', 'Website',9.76, '2025-01-07', '2025-01-11'),
(1006, 'Susan', 'Pencil', 'Website', 10.8, '2025-01-10', '2025-01-13'), 
(1007, 'Robert', 'Pen", 'App', 5.1, '2025-01-13', '2025-01-23'),
(1008, 'Melissa', 'Eraser', 'Website',9.13, '2025-01-13','2025-01-17'), 
(1009, 'David', 'Pencil', 'Website',6.26, '2025-01-13', '2025-01-15'),
(1010, 'Jennifer', 'Marker', 'Store',5.67, '2025-01-16', '2025-01-21'),
(1011, 'Thomas', 'Pen', 'Store',9.04, '2025-01-16', '2025-01-22'),
(1012, 'Lisa', 'Binder', 'App', 7.13, '2025-01-17','2025-01-26'),
(1013, 'Richard', 'Desk', 'Website', 15.2, '2025-01-17', '2025-01-18'),
(1014, 'Matthew', 'Binder', 'Website',9.24, '2025-01-17', '2025-01-18'), 
(1015, 'Charles', 'Envelope', 'Website',6.58, '2025-01-17', '2025-01-18'),
(1016, 'Michelle', 'Envelope', 'Store',9.17, '2025-01-18', '2025-01-21'),
(1017, 'Sandra', 'Envelope', 'App', 6.86, '2025-01-19', '2025-01-23'),
(1018, 'Mary', 'Pencil', 'App', 14.31, '2025-01-22', '2025-01-29'),
(1019, 'Michael', 'Pen Set', 'Website', 7.95, '2025-01-25', '2025-01-29'),
(1020, 'Donald', 'Eraser', 'App', 11.13, '2025-01-26' , '2025-01-28'), 
(1021, 'Amanda', 'Pen Set', 'Website', 12.54, '2025-01-27', '2025-01-29'),
(1022, 'Kenneth', 'Pen Set', 'App', 6.82, '2025-01-31','2025-02-03'),
(1023, 'Timothy', 'Chair', 'Store', 10.82, '2025-02-01','2025-02-06'),
(1024, 'Joshua', 'Desk', 'Website', 11.48, '2025-02-01','2025-02-02'),
(1025, 'Christopher', 'Scissors', 'Website', 6.45, '2025-02-02','2025-02-04'),
(1026, 'Elizabeth', 'Desk', 'Store',15.05,'2025-02-04','2025-02-05'),
(1027, 'Joseph', 'Chair', 'Store', 14.97, '2025-02-05','2025-02-08'),
(1028, 'George', 'Marker', 'Website', 6.76,'2025-02-05','2025-02-07'),
(1029, 'Sarah', 'Pen', 'Store', 14.38, '2025-02-06','2025-02-10'),
(1030, 'Betty', 'Scissors', 'App', 7.21, '2025-02-07','2025-02-11'),
(1031, 'Barbara', 'Pen Set', 'Store', 7.89, '2025-02-11', '2025-02-13'),
(1032, 'Brian', 'Scissors', 'Store', 8.75, '2025-02-12','2025-02-16'),
(1033, 'Jessica', 'Pen Set', 'Website', 9.59, '2025-02-13','2025-02-18'),
(1034, 'Ashley', 'Envelope', 'Store', 6.7, '2025-02-18', '2025-02-21'),
(1035, 'Margaret', 'Binder', 'Website', 14.58, '2025-02-19', '2025-02-22'),
(1036, 'John', 'Marker', 'App',8.24, '2025-02-20', '2025-02-22'),
(1037, 'Kimberly', 'Eraser', 'Website', 10.8,'2025-02-22','2025-02-26'), 
(1038, 'Karen', 'Scissors', 'Store', 5.59,'2025-02-22','2025-02-23'),
(1039, 'Paul', 'Pencil', 'Store', 10.18, '2025-02-28','2025-03-03'),
(1040, 'Donna', 'Marker', 'Website', 11.61, '2025-02-28','2025-03-01'),
(1041, 'Emily', 'Envelope', 'App', 14.2, '2025-03-01','2025-03-04'), 
(1042, 'James', 'Chair', 'Website', 13.13, '2025-03-03','2025-03-06'),
(1043, 'William', 'Eraser', 'App', 13.01, '2025-03-04', '2025-03-09'),
(1044, 'Anthony', 'Chair', 'Website', 11.19, '2025-03-05','2025-03-08'), 
(1045, 'kevin', 'Eraser', 'Store',8.57, '2025-03-14', NULL),
(1046, 'Carol', 'Desk', 'App', 13.88, '2025-03-15', NULL), 
(1047, 'Daniel', 'Desk', 'App', 11.57, '2025-03-16', NULL), 
(1048, 'Mark', 'Marker', 'Store',8.8, '2025-03-19', NULL), 
(1049, 'Nancy', 'Binder', 'App',5.85, '2025-03-20', NULL),
(1050, 'Patricia', 'Binder', 'Website', 12.32, '2025-03-24', NULL);

SELECT product_name, COUNT(orderid) AS "Number of Orders"
SUM(order_amount) AS "Total Sales"
FROM order_details
GROUP BY product_name;

Coding Exercise 2

For the "order_details" table created in previous coding exercise, make the following changes:
	a: Change name of column "customer_name" to "customer_first_name"
	b: Add a new column for "cancel_date"

ALTER TABLE order_details RENAME COLUMN customer_name TO customer_first_name;

ALTER TABLE order_details ADD COLUMN cancel_date DATE;
