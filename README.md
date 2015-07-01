FastORM
=============

A simple dictionary based **Property-Table Column** mapper. Here I used `Sqlite` database. 


* ObjectBuilder 
* MapTable

are the main classes. 


**ObjectBuilder**:  It scan the class and filtered out all *Column*, *Id* and *Table* attributes. An internal dictionary keep all processed information for further use. There are different mapped value stored in it, ex: *Column-Property*, *Column-PropertyType*

**MapTable**: It is a facade of the class-table object. 
