# Challenge

The challenge was to create a transaction file upload, where each line would be stored in the database for later reference.

The focus was not on completing the project as a whole, but rather on creating a robust architecture capable of supporting a very large scale without compromising performance or using excessive computational resources.

Another focus was on code extensibility, maintainability, and testability.

## Architecture
The initial architectural idea for this exercise involves an Angular front-end making requests to a .NET back-end. The .NET application manages its logs by exporting them to a local folder in the form of a structured log file, which is then mapped to a volume managed by Docker Compose. Within the same Docker Compose definition file, we also have Grafana, Grafana Loki, and Promtail, which reads these log files and indexes them for viewing within Grafana. We also use Postgres for data persistence and Minio, which is a bucket that implements the AWS S3 API. 

The idea behind this architecture is to enable production-level scaling. Since we are talking about a transaction file, it can typically have millions, if not billions, of lines. For this reason, we cannot upload directly to the server, to the backend. This can compromise performance and is not the most appropriate case when we are talking about a production system with this type of scale. Therefore, the front-end makes a request to the back-end for a pre-signed URL and, with this pre-signed URL, uploads directly to the bucket, in this case managed by Minio. Next, the back-end application downloads this file, processes it, and persists this data within Postgres.

![image](resources/architecture.png)

## Overengineering

Regarding overengineering, this project is clearly more complex than expected to be solved in a simple way. However, as mentioned earlier, the focus was on a solution that can be used on a large scale of data and is also easy to extend, making it possible to easily add new files to be imported, validated, and tested.

The persistence of files that were imported incorrectly is also added for later reporting or manual processing, whether for verification or adjustments.

## Areas for improvement

It would also be interesting, as an area for improvement, to implement a login. In this case, using an application called Key Cloak, which would be responsible for creating and managing user accounts for the system. The addition of customized metrics such as the number of imported transactions, the number of transactions with errors, all to create a monitoring dashboard within Grafana. Unfortunately, due to the complexity involved, it was not possible to complete the entire implementation in this project within the time allowed.

## How to use
All dependencies are contained in the docker-compose file, requiring docker and the docker-compose CLI to be installed.
Simply run the command `docker-compose up -d` (you can use -d or not, to run attached to the terminal or not).

The front-end application is configured to run on port 4200 and can be accessed in the browser via the URL `http://localhost:4200`.
![image](resources/upload.png)