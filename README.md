# Western University Student Center v3.0 - Backend
## [Front-End Repository](https://github.com/AndrewPitblado/Student_Center_3.0)
## Problem statement
As of October 2024, Western University has a total of 36,205 students who must all use the Student Center to track and handle changes relating to their academics and finances (Western University, 2024). Clearly, the Student Center is a cornerstone of university life, making a good user experience (UX) exceedingly important. Following that logic, the Student Center was given a vast overhaul in late 2022, with the goal of improving mobile UX and allowing students to “access what they need in 3 clicks or less” (Western University, 2022). 
Unfortunately, among the student population, it is not an uncommon opinion that the new Student Center is generally worse than the previous one. Though mobile UX is out of the project scope, this project will address three common device-agnostic frustrations:
### Unintuitive Interface
The user interface (UI) of the current system is poorly designed, organizing features into bins. Upon entering a bin, all other options disappear. This unnecessarily complicates between-feature navigation as users must somewhat memorize what features are located where. For example, if users were in “Course Registration” but wished to see their “Grades”, they must click two buttons (back → “Grades”) rather than navigating to “Grades” directly.

### Ineffective Navigation
The back button is inconsistent in its function. It may bring the user back to the previous page the user accessed, or it may bring the user back to a main heading containing feature bins. When it comes to course enrollment, this inconsistency can allow students to lose their progress. This case proves a particularly problematic flaw, especially for high-demand courses where even a small delay can cause available seats to fill up.

### Bad Schedule-Viewing Defaults
The default view of a user’s classes is in list format rather than the more user-friendly calendar view. This list view also only shows the start times of each class, making it quite useless at giving users a general idea of their free and busy periods. 

## Repository Description and Backend Architecture
This repository compiles the back-end code for this new Student Center, written using the ASP.NET Core Web App API and Entity Framework. For separation of concerns and theoretical scalability, the back-end also uses the Model-View-Controller (MVC) pattern, with the following adapter pattern-inspired architecture:

1. **Student_Center_3.0_Serivces:** this project contains the core of all business logic, henceforth referred to as the Service Layer. Functions as the "adapter" part of the adapter pattern, where information requested/sent by the front-end is changed into a format that the database can use (and vice versa). Consists of two main sub-layers
    - **Services:** classes that perform the business logic to transform data, calls upon API endpoints connecting to the database in order to retrieve and modify information
    - **Controllers:** API endpoints called upon by the front-end.
2. **Student_Center_3.0_Database:** this project is the Database Layer, responsible for directly calling and manipulating the Microsoft SQL Server database tables. Also contains the necessary information to create all needed tables via Entity Framework. Also consists of two main sub-layers
    - **Models:** classes that contain column information for all database tables
    - **Controllers:** API endpoints called upon by the Service Layer to retrieve and modify table information.
