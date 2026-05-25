Add-Type -Path "$PSScriptRoot\TechdriveLogin\bin\Debug\Npgsql.dll"
$connString = "Host=nordic-coyote-16113.jxf.gcp-asia-southeast1.cockroachlabs.cloud;Port=26257;Database=defaultdb;Username=rome;Password=j6fSYFN3UndFa7-smeTaKg;SSL Mode=Require;"
$conn = New-Object Npgsql.NpgsqlConnection($connString)
try {
    Write-Host "Connecting to CockroachDB..." -ForegroundColor Yellow
    $conn.Open()
    Write-Host "Successfully Connected! Querying users table..." -ForegroundColor Green
    $cmd = New-Object Npgsql.NpgsqlCommand("SELECT username, full_name, role FROM users;", $conn)
    $reader = $cmd.ExecuteReader()
    while ($reader.Read()) {
        Write-Host "User: $($reader.GetString(0)) | Name: $($reader.GetString(1)) | Role: $($reader.GetString(2))" -ForegroundColor Green
    }
} catch {
    Write-Host "ERROR Connecting to Database: $_" -ForegroundColor Red
} finally {
    $conn.Close()
}
