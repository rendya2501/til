# Change the working directory to the script's directory
$scriptPath = Split-Path -Parent -Path $MyInvocation.MyCommand.Definition
Set-Location -Path $scriptPath

# Create script blocks for win-x64 and linux-x64 tasks
$win_x64_task = {
    [Console]::OutputEncoding = [System.Text.Encoding]::UTF8
    Set-Location -Path $using:scriptPath
    Write-Output "--- win-x64_Start ---"
    dotnet publish -o ../Bundle/win_bundle -c Release --self-contained true -r win-x64 -p:IncludeNativeLibrariesForSelfExtract=true
    Write-Output "--- win-x64_Finished ---"
}

$linux_x64_task = {
    [Console]::OutputEncoding = [System.Text.Encoding]::UTF8
    Set-Location -Path $using:scriptPath
    Write-Output "--- linux-x64_Start ---"
    dotnet publish -o ../Bundle/linux_bundle -c Release --self-contained true -r linux-x64
    Write-Output "--- linux-x64_Finished ---"
}

# Start the tasks as background jobs
$winJob = Start-Job -ScriptBlock $win_x64_task
$linuxJob = Start-Job -ScriptBlock $linux_x64_task

# Wait for all jobs to complete
Wait-Job -Job $winJob, $linuxJob

# Display the output from the jobs
Receive-Job -Job $winJob
Receive-Job -Job $linuxJob

# Clean up
Remove-Job -Job $winJob, $linuxJob

Write-Output "--- AllFinished ---"
