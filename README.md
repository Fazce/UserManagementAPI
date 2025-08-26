# UserManagementAPI
-----------------------
This is a C# Program that replicates a simple User Management API system and includes CRUD endpoints and validation systems.
It also includes middleware such as logging and authentication. 

# Running the Program
-----------------------
To retrieve the files for this program, go into your terminal and then write down

git clone https://github.com/Fazce/UserManagementAPI

After getting the files from this git repository, build and run the program within a terminal system or IDE of your choice (I recommend Visual Studio Code)
When the program builds and runs sucessfully, a localhost link will be given in within the terminal. Copy and paste the link into your browser of choice, then include "/swagger" near the end so that the Swagger UI will appear. This is necessary for the API to run correctly. Without the use of Swagger (or an alternative like Postman), the localhost site will not load properly. 

# Authorization
----------------------
The endpoints of this API (except for Swagger) will require the need of an authorization token in its header. The authorization token will give told to you when clicking on the "Authorize" button, 
but for simple copy and paste, the token is: Bearer mysecrettoken123

