# ASP.NET Core - BlogAPI
This project is a simple RESTful API for a blog platform. It allows users to perform CRUD (Create, Read, Update, Delete) operations on blog posts. 
Each blog post consists of a title, a body, and a publish date. Additionally, the API supports filtering the list of blog posts by publish date.
Additionally, user authentication and authorization have been implemented to ensure that only authenticated users can create, update, and delete blog posts. 

# Technologies Used
  • ASP.NET Core 7 <br />
  • Entity Framework Core <br />
  • MS SQL
  
 # API Endpoints
  <strong>Auth</strong> <br />
  • POST /api/auth/register: Register a new user. <br />
  • POST /api/auth/login: Authenticate a user and generate a valid token. <br />
  
 <strong>Blog</strong> <br />
  • POST /api/blogs: Create a new blog. <br />
  • GET /api/blogs: Retrieve a list of all blogs, as well as filter the list by publish date. <br />
  • GET /api/blogs/{id}: Retrieve details of a specific blog. <br />
  • PUT /api/blogs/{id}: Update the details of a blog. <br />
  • DELETE /api/blogs/{id}: Delete a blog. <br />
