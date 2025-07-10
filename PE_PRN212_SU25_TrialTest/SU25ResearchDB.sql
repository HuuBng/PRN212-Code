USE master;
GO

CREATE DATABASE SU25ResearchDB;
GO

USE SU25ResearchDB;
GO

/*
UserAccount Table:
- UserID: Unique identifier for each user.
- Password: Encrypted password for authentication.
- Email: Unique email for user login.
- Role: Defines user roles (1: Admin, 2: Manager, 3: Staff, 4: Member).
- CreatedDate: Timestamp of account creation.
*/
CREATE TABLE UserAccount (
  UserID INT PRIMARY KEY,
  Password NVARCHAR(60) NOT NULL,
  Email NVARCHAR(100) UNIQUE NOT NULL,
  Role INT CHECK (Role BETWEEN 1 AND 4),
  CreatedDate DATETIME DEFAULT GETDATE()
);
GO

INSERT INTO UserAccount VALUES (101, N'@1', 'admin@research.com', 1, GETDATE());
INSERT INTO UserAccount VALUES (102, N'@1', 'manager@research.com', 2, GETDATE());
INSERT INTO UserAccount VALUES (103, N'@1', 'staff@research.com', 3, GETDATE());
INSERT INTO UserAccount VALUES (104, N'@1', 'member@research.com', 4, GETDATE());
GO

/*
Researcher Table:
- ResearcherID: Unique identifier for each researcher.
- FullName: Researcher's full name.
- Affiliation: Institution or organization the researcher is associated with.
- Email: Unique email address for contact.
- Expertise: Areas of specialization or research focus.
*/
CREATE TABLE Researcher (
  ResearcherID INT PRIMARY KEY,
  FullName NVARCHAR(100) NOT NULL,
  Affiliation NVARCHAR(150) NOT NULL,
  Email NVARCHAR(100) UNIQUE NOT NULL,
  Expertise NVARCHAR(200) NOT NULL
);
GO

INSERT INTO Researcher VALUES (201, N'Michael Johnson', N'University of Science', 'michael@research.edu', N'AI, Machine Learning');
INSERT INTO Researcher VALUES (202, N'Susan Adams', N'Institute of Technology', 'susan@research.edu', N'Big Data, NLP');
INSERT INTO Researcher VALUES (203, N'James Carter', N'Technical University', 'james@research.edu', N'Blockchain, Cryptography');
INSERT INTO Researcher VALUES (204, N'David Brown', N'National University', 'david@research.edu', N'IoT, Edge Computing');
GO

/*
ResearchProject Table:
- ProjectID: Unique identifier for each research project.
- ProjectTitle: Title of the research project.
- ResearchField: The domain or category of the research.
- StartDate: Official start date of the project.
- EndDate: Expected completion date.
- LeadResearcherID: Researcher leading the project (FK to Researcher table).
- Budget: Total allocated budget for the project (must be non-negative).
*/
CREATE TABLE ResearchProject (
  ProjectID INT PRIMARY KEY,
  ProjectTitle NVARCHAR(200) NOT NULL,
  ResearchField NVARCHAR(100) NOT NULL,
  StartDate DATE NOT NULL,
  EndDate DATE NOT NULL,
  LeadResearcherID INT FOREIGN KEY REFERENCES Researcher(ResearcherID) ON DELETE CASCADE ON UPDATE CASCADE,
  Budget DECIMAL(18,2) NOT NULL CHECK (Budget >= 0)
);
GO

INSERT INTO ResearchProject VALUES (301, N'AI-based Medical Image Analysis System', N'AI & Computer Vision', '2024-01-01', '2025-12-31', 201, 50000.00);
INSERT INTO ResearchProject VALUES (302, N'Blockchain for Supply Chain Management', N'Blockchain', '2023-06-01', '2025-05-31', 203, 75000.00);
INSERT INTO ResearchProject VALUES (303, N'Chatbot for Education using NLP', N'NLP', '2024-02-15', '2026-02-15', 202, 60000.00);
INSERT INTO ResearchProject VALUES (304, N'IoT in Smart Agriculture', N'IoT', '2023-08-01', '2025-08-01', 204, 45000.00);
INSERT INTO ResearchProject VALUES (305, N'Quantum Computing for Cryptography', N'Quantum Computing', '2024-05-01', '2027-04-30', 203, 90000.00);
INSERT INTO ResearchProject VALUES (306, N'Autonomous Vehicles using Deep Learning', N'AI & Robotics', '2023-09-01', '2026-09-01', 201, 85000.00);
INSERT INTO ResearchProject VALUES (307, N'5G Networks and Edge Computing', N'Networking', '2024-03-10', '2026-12-10', 204, 70000.00);
INSERT INTO ResearchProject VALUES (308, N'Cybersecurity Threat Detection with AI', N'Cybersecurity', '2023-11-15', '2026-11-15', 204, 65000.00);
GO
