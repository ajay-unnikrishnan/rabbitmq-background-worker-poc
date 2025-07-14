# 🐇 RabbitMQ Installation & Setup

This guide explains how to install and configure RabbitMQ for local development.

---

## ✅ Option 1: Using Docker (Recommended if Docker is Installed)

If you have Docker installed, you can start RabbitMQ with a single command.

### 🐳 Step 1: Run RabbitMQ with Management Plugin

```bash
docker run -d --hostname my-rabbit --name rabbitmq \
  -p 5672:5672 -p 15672:15672 \
  rabbitmq:3-management
```

- 🟢 **5672** → AMQP protocol (used by your application)
- 🌐 **15672** → Web UI for management plugin

> This runs RabbitMQ in a container with the management UI enabled.

### 🔐 Default Login Credentials

- **URL**: http://localhost:15672  
- **Username**: `guest`  
- **Password**: `guest`

### 🛑 To Stop the Container

```bash
docker stop rabbitmq
```

### ❌ To Remove the Container

```bash
docker rm rabbitmq
```

---

## 🛠️ Option 2: Manual Installation (If Docker is Not Installed)

Use this method if you're working in a native Windows environment without Docker.

---

### 1. Install Erlang/OTP

Download and install Erlang from:

**📥 Erlang Download**: `<location>`

> Ensure the version is compatible with RabbitMQ:  
> [https://www.rabbitmq.com/which-erlang.html](https://www.rabbitmq.com/which-erlang.html)

---

### 2. Install RabbitMQ

Download and install RabbitMQ from:

**📥 RabbitMQ Download**: `<location>`

> Install RabbitMQ **after** installing Erlang.

---

### 3. Add RabbitMQ to the PATH

To use RabbitMQ CLI commands globally:

#### Steps:

1. Press `Windows + S`, search for **Environment Variables**
2. Open **Edit the system environment variables**
3. Click **Environment Variables...**
4. Under **System variables**, select `Path` → click **Edit**
5. Add:

   ```
   C:\Program Files\RabbitMQ Server\rabbitmq_server-<version>\sbin
   ```

6. Click **OK** and restart your terminal

---

### 4. Install and Start RabbitMQ as a Windows Service

Open **Command Prompt as Administrator**, then run:

```cmd
rabbitmq-service install
```

> Registers RabbitMQ as a Windows service

```cmd
rabbitmq-service start
```

> Starts the RabbitMQ background service

---

### 🔁 Alternative Commands to Stop/Start Service

#### Option 1: RabbitMQ CLI

```cmd
rabbitmq-service stop
rabbitmq-service start
```

#### Option 2: Windows Native Service Control

```cmd
net stop RabbitMQ
net start RabbitMQ
```

---

### 5. Fixing Cookie Mismatch Error (If Applicable)

If you see this error:

```
Error: Unable to connect to node rabbit@<hostname>: nodedown
```

Follow these steps:

1. Open:  
   `C:\Users\<YourUser>\.erlang.cookie`

2. Copy the contents

3. Open (in admin mode):  
   `C:\Windows\System32\config\systemprofile\.erlang.cookie`

4. Paste the copied content

5. Set appropriate permissions

6. Restart the RabbitMQ service

---

### 6. Enable the RabbitMQ Management Plugin

Run the following:

```cmd
rabbitmq-plugins enable rabbitmq_management
```

---

### 7. Check RabbitMQ Status

Run:

```cmd
rabbitmqctl status
```

---

### 8. Access RabbitMQ Management UI

- **URL**: http://localhost:15672  
- **Username**: `guest`  
- **Password**: `guest`

> Note: The default `guest` user can only log in from `localhost`.

---


## 9. 🔁 Stopping and Starting RabbitMQ Service

Use either of the following methods to stop or restart the RabbitMQ service:

#### Method 1: Using RabbitMQ CLI

```cmd
rabbitmq-service stop
rabbitmq-service start
```

