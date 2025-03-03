# ChatChitLocal

ChatChitLocal is a privacy-focused chat application consisting of two complementary projects: a WebSocket server built with NestJS and a client application developed in C# WPF. Together, they provide a secure, local messaging solution that prioritizes user privacy by keeping all data on your device.

## Repository Structure

This repository contains two main projects:

1. **WebSocket Server** - A NestJS-based backend that handles message routing and communication
2. **WebSocket Client** - A C# WPF desktop application that provides the user interface

## WebSocket Server (NestJS)

The server component manages WebSocket connections, authentication, and message distribution.

### Technology Stack

- NestJS framework
- Node.js runtime
- WebSocket protocol implementation
- pnpm package manager

### Setup and Installation

```bash
# Navigate to the server directory
cd server

# Install dependencies using pnpm
pnpm install

# Start the development server
pnpm run start:dev

# Build for production
pnpm run build
pnpm run start:prod
```

### Key Features

- Real-time message broadcasting
- User authentication and session management
- Configurable to run on local network only
- Low latency communication

## WebSocket Client (C# WPF)

The client application provides a user-friendly interface for sending and receiving messages.

### Technology Stack

- C# programming language
- Windows Presentation Foundation (WPF)
- .NET Framework/Core
- WebSocket client implementation

### Setup and Installation

```bash
# Navigate to the client directory
cd client

# Open the solution file in Visual Studio
# or build using the .NET CLI:
dotnet restore
dotnet build
dotnet run
```

### Key Features

- Modern, intuitive user interface
- Message encryption
- Chat history stored locally
- Support for multiple chat rooms

## Getting Started

To set up the full ChatChitLocal system:

1. Start the WebSocket server:
   ```bash
   cd server
   pnpm install
   pnpm run start
   ```

2. Launch the client application:
   - Open the client solution in Visual Studio and run the application
   - Or use the .NET CLI as described above

3. Connect to the server using the client application:
   - Enter the server address (default: `ws://localhost:3000`)
   - Create or join a chat room
   - Begin secure, local messaging

## Development

### Prerequisites

- Node.js (v14 or higher)
- pnpm package manager
- .NET SDK (v6.0 or higher)
- Visual Studio 2019 or newer (recommended for client development)

### Contributing

Contributions to either the server or client components are welcome:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

Please specify which project your contribution targets in the PR description.

## Security

ChatChitLocal implements several security measures:

- End-to-end encryption for messages
- Local-only deployment option
- No external data storage
- Authentication for room access

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Contact

ThanhTNV - [GitHub Profile](https://github.com/ThanhTNV)

Project Link: [https://github.com/ThanhTNV/ChatChitLocal](https://github.com/ThanhTNV/ChatChitLocal)
