# OpenAI Python examples

The purpose of this folder is to have examples of OpenAI python related code.

## Running this on Linux

In order to run this example make sure `python3` is install on your machine.

```bash
  
    # 1. in the root of this repo run set your openai api key
    export API_KEY=

    # 2. create python virtual environment
    python3 -m venv .venv

    # 3. launch vscode
    code-insiders .


    # 4. in the vscode terminal
    python3 -m pip install -r requirements.txt
```

## [constitution-qa-bert-local](constitution-qa-bert-local/app.py)

Local example of Constitution QA app running BERT model.

```bash
    # install deps after creating virtual environment
    python3 -m pip install -r /constitution-qa-bert-local/requirements.txt

    # run the example

    cat constitution-qa-bert-local/requirements.txt | xargs -n 1 python3 -m pip uninstall -y
```
