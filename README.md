# PC Mate

PC Automation with voice control and HomeAssistant (or other exposed endpoint)

## Installation

Requirements:
* An internet exposed endpoint (such as required for HomeAssistant Alexa/Google assistant integrations)
* A windows device and some way of communicating with Amazon Alexa
* An Amazon developer account (Free)

Let's begin, 

**Automation**:
* If you are using HASS, first add these lines to your configuration.yaml file and restart HomeAssistant.

```
shell_command:
  forwardjson: >-
    curl -X POST -H 'Content-Type: application/json' -d '{{ data }}' {{ url }}
```

* Create a new automation under Configuration -> Automations. Set the trigger type to webhook and generate a long secure webhook ID. At the bottom add a new action, click the 3 dots on the top right of this card and select 'Edit as YAML', paste in the following while replacing the IP with the IP of computer you want to automate tasks on, its highly recommended to set a static IP for this computer.

```
data_template:
  data: '{{trigger.json}}'
  url: 'http://IP:6667'
service: shell_command.forwardjson
```
* Save

**Alexa Skill**:
* Create/Log in to your Alexa developer console at https://developer.amazon.com/alexa/console/ask 
* Hit Create skill, enter a skill name, under 'Choose a model to add to your skill' select "Custom" and under "Choose a method to host your skill's backend resources" select "Alexa-Hosted (Python)"

* Under "Choose a template to add to your skill" select "Hello world skill", let it create the skill.
* Under "Interaction model" click "JSON Editor" and paste the JSON code from [AmazonSkill](AmazonSkill.txt) 
 in this repository
* Click Build model and let it build
* Go to the "Code" page and replace all the code in "lambda_function.py" with the code from [AmazonLambdaFunction](AmazonLambdaFunction.txt) 
 in this repository while changing the "base_url" value to your HomeAssistant webhook endpoint from the automation.
* Click Deploy and let it finish.
* Head over to the "Test" page and select "Development" instead of "Off" in the dropdown menu at the top.



**PC Mate**:
* Download and unzip the latest release from the releases page and run it.



## Usage

Using PCMate you can create flows to do various automations based on voice commands.


Flows are made up of an action or multiple actions which are executed in the order they were created
* To create a new flow press the "+" button at the bottom right corner
* Choose an intent from the dropdown list, and fill in an utterance (can be anything you want up to 4 words)
* Press Actions + to create new actions, they will be executed in the order they were created when a matching voice command is received.


Current supported intents:
* Turn on / Turn off
* Switch on / Switch off
* Activate / Deactivate
* Increase / Decrease
* Adjust / Change
* Mute
* Set
* Launch
* Stop / Play
* Close / Open
* Lock / Unlock
* Enable / Disable
* Reset / Restart / Reboot / Reload
* Connect / Disconnect
* Reconnect

Current supported utterences:
* Any utterance, up to 4 words.

Current supported actions:
* Open Program
* Set volume by voice or manual override
* Increase/Decrease volume by voice or manual override
* Change audio device
* Turn on/off screen
* Shut down computer / Sleep / Hibernate
* Fire GET webhook
* Media Keys - Play/Pause/Next/Previous
* Delay


Example voice commands:

Ask auto to:
* Turn off my computer screen
* Play music
* Open Chrome
* Open Youtube
* Set volume to 50
* Increase volume by 10

Upcoming (Planned):
* Play music for 30 minutes
* Turn off my computer in 30 minutes


## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

## Donation
If this project helped your life in any way and you want to support its development

[![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=H47Y39R579B6Y&currency_code=USD&source=url)

## License
[GPL 3.0]

## To-do list
* JSON DB Updater
* Support for google assistant
* Disable/enable specific flows
* Scheduled actions
* Mute specific programs
* Restart computer action


Tentative:
* Multiple computer support - currently is available per amazon account 
* Add additional webhook actions - POST/PUT, headers, content
* Direct hass integration? - currently can be done with the webhook action and automation/node-red
