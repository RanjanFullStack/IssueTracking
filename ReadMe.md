# IssueTracking Project

## Synopsis

The IssueTracking project is a comprehensive issue tracking system designed to help teams manage and track issues efficiently. It provides features for creating, updating, and managing issues, projects, and tags. The system is built using .NET and follows best practices for software development.

## How to Create an Admin User

To create an admin user, you can use the following command:

```sh
dotnet user-jwts create --name "admin" --role "Admin"
```

This command will generate a JWT for the user "admin" with the role "Admin". The token can be used for authentication and authorization within the system.

## Getting Started

To get started with the IssueTracking project, follow these steps:

1. Clone the repository:
   ```sh
   git clone https://github.com/your-repo/IssueTracking.git
   ```

2. Navigate to the project directory:
   ```sh
   cd IssueTracking
   ```

3. Restore the dependencies:
   ```sh
   dotnet restore
   ```

4. Build the project:
   ```sh
   dotnet build
   ```

5. Run the project:
   ```sh
   dotnet run
   ```

6. Open your browser and navigate to `http://localhost:5000` to access the application.

## Contributing

We welcome contributions from the community. To contribute to the IssueTracking project, please follow these guidelines:

1. Fork the repository.
2. Create a new branch for your feature or bug fix.
3. Make your changes and commit them with descriptive commit messages.
4. Open a pull request against the `main` branch.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.
