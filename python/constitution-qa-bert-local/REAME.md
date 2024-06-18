# US Constitution

## Running Locally

To run the application locally, follow these steps:

1. **Create a virtual environment:**

    ```bash
    python -m venv .venv
    ```

2. **Activate the virtual environment:**

    ```bash
    source .venv/bin/activate
    ```

3. **Install the dependencies management tool:**

    ```bash
    pip install pip-tools
    ```

4. **Compile the requirements:**

    ```bash
    pip-compile requirements.in
    ```

5. **Install the dependencies:**

    ```bash
    pip install -r requirements.txt
    ```

6. **Run the fask app:**
    ```bash
    flask run --host=0.0.0.0
    ```
    