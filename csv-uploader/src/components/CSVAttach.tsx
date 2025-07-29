


import React, { useState } from "react";
import { json } from "stream/consumers";

const CSVAttach: React.FC = () => {
  const [file, setFile] = useState<File | null>(null);
  const [message, setMessage] = useState("");
  const [tableData, setTableData] = useState<Array<Record<string, string>>>([]);
  const [showDetailsButton, setShowDetailsButton] = useState(false);
  const [showTable, setShowTable] = useState(false);
  const [tableName,setTableName] = useState('')
  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files.length > 0) {
      setFile(e.target.files[0]);
    }
  };

  const handleUpload = async () => {
    if (!file) {
      setMessage("Please select a CSV file first.");
      return;
    }

    const formData = new FormData();
    formData.append("file", file);

    try {
      const response = await fetch("http://localhost:5001/api/CsvUpload/upload", {
        method: "POST",
        body: formData,
      });

      if (response.ok) {
        const data = await response.json();
        setMessage(`Upload successful! Rows processed: ${data.rowsInserted}`);
        setTableName(data.tableName); // store the uploaded table name
        setShowDetailsButton(true);
      } else {
        setMessage("Upload failed.");
        setShowDetailsButton(false);
      }
    } catch (error) {
      setMessage("Error uploading file.");
      console.error(error);
      setShowDetailsButton(false);
    }
  };


  const fetchUploadedData = async () => {
  if (!tableName) {
    setMessage("No table name found.");
    return;
  }

  try {
    const response = await fetch(`http://localhost:5001/api/CsvUpload/data?tableName=${tableName}`);
    if (response.ok) {
      const data = await response.json();
      setTableData(data);  // store for table rendering
    } else {
      setMessage("Failed to fetch data.");
    }
  } catch (error) {
    setMessage("Error fetching data.");
    console.error(error);
  }
};

  const handleShowDetails = () => {
    setShowTable(true); // Show the table when button clicked
  };

  const handleToggleDetails = () => {
  setShowTable(!showTable);
};

console.log(tableName,"tab ")

return (
    <div>
      <h2>Upload CSV File</h2>
      <input type="file" accept=".csv" onChange={handleFileChange} />
      <button onClick={handleUpload}>Upload</button>
      {message && <p>{message}</p>}

      {/* Conditionally show "Details" button */}
      {showDetailsButton && (
        <button onClick={handleShowDetails} style={{ marginTop: "10px" }}>
          Show Details
        </button>
      )}

<button onClick={handleToggleDetails}>
  {showTable ? "Hide Details" : "Show Details"}
</button>

      {/* Conditionally show table */}
      {showTable && tableData.length > 0 && (
        <table border={1} style={{ marginTop: "15px" }}>
          <thead>
            <tr>
              {Object.keys(tableData[0]).map((header) => (
                <th key={header}>{header}</th>
              ))}
            </tr>
          </thead>
          <tbody>
            {tableData.map((row, rowIndex) => (
              <tr key={rowIndex}>
                {Object.values(row).map((value, colIndex) => (
                  <td key={colIndex}>{value}</td>
                ))}
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
};

export default CSVAttach;
