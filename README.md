## **1\. Overview**

This project implements a RESTful API for a Survey Tool. The system API allows users to create and manage surveys, questions, and responses. A score for a survey response can also be retreived using one of the endpoints.

This repository includes two main components:

1. **Survey Tool API:** A backend built using C\# and .NET 8.  
2. **Survey Frontend:** A webpage built using React Typescript

## **2\. Technical Stack**

| Component | Technology | Framework/Library |
| :---- | :---- | :---- |
| **Backend API** | C\# | .NET 8, ASP.NET Core Web API |
| **Data Store** | In-Memory | Entity Framework Core (In-Memory Provider) |
| **API Documentation** | OpenAPI | Swagger UI |
| **Frontend** | TypeScript | React |
| **Styling** | CSS |  |

## **3\. How to Run and Test the API**

### **Prerequisites**

You must have the following software installed:

* **.NET 8 SDK** or newer  
* **Node.js** (LTS version) and **npm**

### **3.1. API Setup (C\# Backend)**

1. **Navigate to the API directory:**  
   cd \<project-root\>/SurveyTool

2. **Restore dependencies:**  
   dotnet restore

3. **Run the application:**  
   dotnet run

The API will run on "https://localhost:7212" and "http://localhost:5268"

### **3.2. Testing the API with Swagger UI**

Once the API is running, you can access the Swagger UI for interactive testing and documentation:

* **Swagger URL:** https://localhost:7212/swagger/index.html

Using Swagger you can access all API routes to create, update, delete, and get information. This should be used to add the initial surveys that can be accessed from the frontend.

### **3.3. Frontend Setup (React/TypeScript)**

1. **Navigate to the Frontend directory:**  
   cd \<project-root\>/SurveyFrontend

2. **Install dependencies:**  
   npm install  

3. **Start the development server:**  
   npm run start

The frontend application will open at **http://localhost:3000**. It is configured to interact with the some of the API endpoints.

## **4\. Architectural Decisions**

The API is structured following the principles of **Clean Architecture** (or a simple layered approach) to ensure separation of concerns, testability, and maintainability.

### **A. Layers Implemented**

1. **API/Presentation Layer:** This receieves incoming HTTP requests (GET, POST, PUT, DELETE), handles routing and ensures the data coming in is correct. Utilizes ASP.NET Controllers (SurveyController) to handle the requests.  
2. **Business Logic/Service Layer:** This layer contains all the rules for managing data and sending survey response scores. Uses SurveyDataTools file to store this logic.
3. **Data/Persistence Layer:** This section interacts with the data storage. Using Entity Framework Core, and the In-Memory database data can be managed with the help from the SurveyDbContext file.

### **B. In-Memory Data Store**

The use of EF Core In-Memory provided an ease to set up a quick DB to set up this project. If wished to be expanded on this coould easily be hooked up to a real Database and transactions could be implemented to further enhance the durability of the program.

### **C. Error Handling and Validation**

* **Validation:** Data received via requests is validated before it can alter or any data in the DB.  
* **Error Responses:** Proper HTTP status codes are used for responses (e.g., 200 OK, 201 Created, 204 No Content, 400 Bad Request, 404 Not Found, and 500 Internal Server Error).  

## **5\. Scoring Implementation**

The scoring system is designed to be simple based on the requirements, and the logic is implemented within the SurveyDataTools.CalculateResponseScoreAsync method.

### **Score Calculation:**

The method reutrns the score as a ratio of the response score vs the maximum possible score: "{responseScore}/{maxScore}"

### **Logic Details:**

1. **Weights:** Only Answer entities carry a Weight (a numeric value).  
2. **Response Processing:** The CalculateResponseScoreAsync method retrieves the user's selected answers (TblResponseAnswer).  
3. **Achieved Score Calculation:** It iterates through the selected answers and sums up the Weight from the corresponding TblAnswer entity to determine the achievedScore.  
4. **Maximum Score Calculation:** The logic also calculates the potential maxScore for the entire survey based on the question type:  
   * **Single Choice (TypeId \= 1):** The maximum score for that question is the **maximum** weight among all possible answers.  
   * **Multiple Choice (TypeId \= 2):** The maximum score for that question is the **sum** of all weights among all possible answers (assuming all correct/weighted answers can be selected).  
5. **Result:** The method returns the score as a string: "{achievedScore}/{maxScore}".

This design ensures that only specific, weighted answers contribute to the score, and the scoring calculation is tailored to the expected behavior of single-select vs. multi-select questions. Free text questions are not counted towards scoring.

## **6\. Data Model / Class Relationships**

The core entities are designed with a clear relationship structure:

| Entity | Description | Key Relationships |
| :---- | :---- | :---- |
| **Survey** | The main container. | One-to-many relationship with Question. |
| **Question** | A component of a Survey. Has a QuestionType (e.g., Single, Multiple, Text). | One-to-many relationship with Answer. |
| **Answer** | A potential option for a Question, contains the scoring information. | Has a numeric property: Weight. |
| **Response** | A user's submission to a Survey. | Many-to-one relationship with Survey. |
| **ResponseAnswer** | Links a Response to the specific Answers selected. | Many-to-one relationship with Response and Answer. |

This structure allows for handling both free text, single, and multiple-choice questions effectively, as a single response can be associated with multiple answers.

## **7\. Assumptions Made**

1. **Authentication/Authorization:** There is no user authentication. All API endpoints are publicly accessible.  
2. **Data Typing:** All answer weights are assumed to be simple integers **numeric values**.  
3. **Free Text Scoring:** Free Text questions are assumed to have a Weight of 0.  
4. **Minimal APIs vs. Controllers:** The API uses **traditional ASP.NET Core Controllers** for endpoint definition (e.g., SurveyController).  
5. **Initial Data:** The application **does not** include initial data upon startup. Surveys must be created via the API before responses can be submitted.
6. **Transactions:** The API does not use transactions as it was limited by the In-Memory Database.
7. **Persistence:** All data is lost when the API is stopped.
