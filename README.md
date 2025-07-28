# That Integration Engine

A local system process management platform designed as a thought project. This engine provides dynamic process invocation and lifecycle management, processing work requests and spawning job members as needed.

## Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Core Components](#core-components)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Usage Examples](#usage-examples)
- [Development](#development)
- [License](#license)

## Overview

The That Integration Engine is a sophisticated process management system that:

- **Dynamically invokes processes** based on configured triggers
- **Manages process lifecycles** from initiation to completion
- **Handles work requests** through a configurable pipeline
- **Spawns job members** automatically as workload demands
- **Provides multiple trigger mechanisms** including file watchers and cron schedulers
- **Supports extensible process definitions** through custom implementations

## Architecture

The system follows a modular, event-driven architecture with the following key patterns:

### Core Design Principles

1. **Separation of Concerns**: Core process logic separated from service orchestration
2. **Plugin Architecture**: Extensible handlers and process definitions
3. **Event-Driven Processing**: Trigger-based process activation
4. **Configuration-Driven**: XML-based configuration for processes and relationships
5. **Scalable Execution**: Dynamic spawning of process instances

### High-Level Flow

```
Triggers (File Watchers/Schedulers) → Engine → Process Manager → Process Execution → Results
```

## Project Structure

```
src/
├── ThatIntegrationEngine.sln              # Main solution file
├── Core/                                   # Core engine components
│   ├── Components/
│   │   ├── Adapters/                      # File I/O adapters
│   │   │   ├── FileReader.cs              # File reading utilities
│   │   │   └── TieFileInfo.cs             # File information wrapper
│   │   └── Handlers/                      # Event handlers
│   │       ├── Handler.cs                 # Base handler implementation
│   │       ├── IHandler.cs                # Handler interface
│   │       ├── Schedulers/                # Cron-based scheduling
│   │       │   ├── ICronScheduler.cs      # Scheduler interface
│   │       │   ├── ICronSchedulerCollection.cs
│   │       │   └── Impl/
│   │       │       ├── CronScheduler.cs   # Quartz.NET scheduler implementation
│   │       │       └── CronSchedulerCollection.cs
│   │       └── Watchers/                  # File system watchers
│   │           ├── IDirectoryWatcher.cs   # Directory watcher interface
│   │           ├── IDirectoryWatcherCollection.cs
│   │           └── Impl/
│   │               ├── DirectoryWatcher.cs    # FileSystemWatcher implementation
│   │               └── DirectoryWatcherCollection.cs
│   ├── Process/                           # Process management
│   │   ├── ITieProcess.cs                 # Main process interface
│   │   ├── IProcessDetails.cs             # Process metadata interface
│   │   ├── Arguments/                     # Process arguments
│   │   │   ├── IArguments.cs              # Base arguments interface
│   │   │   ├── Arguments.cs               # Base arguments implementation
│   │   │   ├── SchedulerArguments.cs      # Scheduler-specific arguments
│   │   │   └── WatcherArguments.cs        # Watcher-specific arguments
│   │   ├── ExecutionResults/              # Process execution tracking
│   │   │   ├── IExecuteResults.cs         # Results interface
│   │   │   ├── ExecuteResults.cs          # Results implementation
│   │   │   └── ExecutionState.cs          # Execution state enumeration
│   │   └── Impl/
│   │       ├── TieProcess.cs              # Main process implementation
│   │       └── ProcessDetails.cs          # Process metadata implementation
│   ├── ExampleHandlers.xml                # Sample handler configurations
│   ├── ExampleProcessDetailRelations.xml  # Sample process definitions
│   └── ExampleTriggerProcessRelations.xml # Sample trigger mappings
├── Services/                              # Service layer and engine
│   ├── Workers/
│   │   └── Engine.cs                      # Main engine orchestrator
│   ├── Overseers/
│   │   └── MasterDistributor.cs           # High-level process distribution
│   ├── RelationLoaders/                   # Configuration loading
│   │   ├── IRelationLoader.cs             # Configuration loader interface
│   │   ├── IEngineSettings.cs             # Engine settings interface
│   │   └── Impl/
│   │       ├── FromXMLRelationLoader.cs   # XML configuration loader
│   │       └── ADORelationLoader.cs       # Database configuration loader
│   └── SettingsElements/                  # Configuration model classes
│       ├── EngineSettingsSection.cs       # Engine configuration
│       ├── ProcessBaseSettingsSection.cs  # Process base settings
│       └── EmailElement.cs                # Email notification settings
├── ProcessTests/                          # Process implementation examples
│   ├── Import/
│   │   ├── CsvImport.cs                   # CSV import process example
│   │   └── XmlImport.cs                   # XML import process example
│   └── Export/
│       ├── CsvExport.cs                   # CSV export process example
│       └── XmlExport.cs                   # XML export process example
└── ServiceTests/                          # Integration tests and examples
    └── Program.cs                         # Example usage and testing
```

## Core Components

### 1. Engine ([Services/Workers/Engine.cs](src/Services/Workers/Engine.cs))

The central orchestrator that:
- Manages process lifecycles
- Coordinates between triggers and processes
- Handles process execution states
- Provides start/stop/restart capabilities

**Key Methods:**
- `Start()` - Initializes and starts the engine
- `Stop()` - Gracefully shuts down all processes
- `Restart(IRelationLoader)` - Restarts with new configuration

### 2. Process Management ([Core/Process/](src/Core/Process/))

#### ITieProcess Interface
Defines the contract for all processes:
- `Execute(TArgs args)` - Main execution method
- `Dispose()` - Resource cleanup

#### Process Implementation
- **TieProcess**: Base implementation handling common functionality
- **ProcessDetails**: Metadata about process execution requirements
- **ExecuteResults**: Tracks execution status and outcomes

### 3. Trigger System

#### File Watchers ([Core/Components/Handlers/Watchers/](src/Core/Components/Handlers/Watchers/))
- Monitor file system changes
- Trigger processes based on file events (create, modify, delete)
- Support for multiple directory monitoring

#### Schedulers ([Core/Components/Handlers/Schedulers/](src/Core/Components/Handlers/Schedulers/))
- Cron-based scheduling using Quartz.NET
- Time-based process triggering
- Support for complex scheduling patterns

### 4. Configuration System

#### Relation Loaders ([Services/RelationLoaders/](src/Services/RelationLoaders/))
- **FromXMLRelationLoader**: Loads configuration from XML files
- **ADORelationLoader**: Loads configuration from databases
- Supports dynamic reconfiguration

## Getting Started

### Prerequisites

- .NET Framework 4.5+
- Visual Studio 2015 or later
- Quartz.NET (for scheduling)

### Building the Solution

1. Open [`src/ThatIntegrationEngine.sln`](src/ThatIntegrationEngine.sln) in Visual Studio
2. Restore NuGet packages
3. Build the solution (Ctrl+Shift+B)

### Basic Usage

```csharp
// Initialize the engine with XML configuration
var relationLoader = new FromXMLRelationLoader("path/to/config.xml");
var engine = new Engine(relationLoader);

// Start the engine
engine.Start();

// Engine will now monitor for triggers and execute processes
// Stop when done
engine.Stop();
engine.Dispose();
```

## Configuration

The system uses XML configuration files to define:

1. **Handlers** ([ExampleHandlers.xml](src/Core/ExampleHandlers.xml))
   - File watchers and their target directories
   - Cron schedulers and their timing patterns

2. **Process Details** ([ExampleProcessDetailRelations.xml](src/Core/ExampleProcessDetailRelations.xml))
   - Process implementations and their parameters
   - Execution requirements and constraints

3. **Trigger-Process Relations** ([ExampleTriggerProcessRelations.xml](src/Core/ExampleTriggerProcessRelations.xml))
   - Mapping between triggers (handlers) and processes
   - Argument passing configurations

### Example Configuration Structure

```xml
<!-- Handler Configuration -->
<handlers>
  <fileWatcher id="ImportWatcher" path="C:\Import" filter="*.csv" />
  <cronScheduler id="NightlyExport" expression="0 0 2 * * ?" />
</handlers>

<!-- Process Configuration -->
<processes>
  <process id="CsvImporter" assembly="ProcessTests.dll" 
           type="ProcessTests.Import.CsvImport" />
</processes>

<!-- Trigger-Process Relations -->
<relations>
  <relation triggerId="ImportWatcher" processId="CsvImporter" />
</relations>
```

## Usage Examples

### Example 1: File Processing Pipeline

Create a process that monitors a directory for new files and processes them:

```csharp
public class FileProcessor : ITieProcess<WatcherArguments>
{
    public IExecuteResults Execute(WatcherArguments args)
    {
        var fileInfo = args.FileInfo;
        
        // Process the file
        ProcessFile(fileInfo.FullName);
        
        return new ExecuteResults
        {
            State = ExecutionState.Success,
            Message = $"Processed {fileInfo.Name}"
        };
    }
    
    private void ProcessFile(string filePath)
    {
        // Your file processing logic here
    }
}
```

### Example 2: Scheduled Data Export

Create a scheduled process that exports data nightly:

```csharp
public class NightlyExporter : ITieProcess<SchedulerArguments>
{
    public IExecuteResults Execute(SchedulerArguments args)
    {
        try
        {
            // Export data logic
            ExportData();
            
            return new ExecuteResults
            {
                State = ExecutionState.Success,
                Message = "Data export completed successfully"
            };
        }
        catch (Exception ex)
        {
            return new ExecuteResults
            {
                State = ExecutionState.Failed,
                Message = ex.Message
            };
        }
    }
}
```

## Development

### Extending the System

1. **Custom Processes**: Implement `ITieProcess<TArgs>` interface
2. **Custom Handlers**: Extend the `Handler` base class
3. **Custom Arguments**: Implement `IArguments` interface
4. **Custom Loaders**: Implement `IRelationLoader` interface

### Testing

- **ProcessTests**: Contains example process implementations
- **ServiceTests**: Contains integration tests and usage examples
- Run [`ServiceTests/Program.cs`](src/ServiceTests/Program.cs) for a complete example

### Architecture Notes

- **Thread Safety**: Engine manages concurrent process execution
- **Resource Management**: Processes should implement proper disposal
- **Error Handling**: Failed processes are logged and don't crash the engine
- **Scalability**: Engine can handle multiple concurrent process instances

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
