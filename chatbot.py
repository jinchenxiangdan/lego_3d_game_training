# Author: Shawn Jin
# Python version 3.7.11
# install pandas, scapy

from chatterbot import ChatBot
from chatterbot.trainers import ListTrainer
# import pandas

chatbot = ChatBot('Test')

conversation = [
    "Hello",
    "Hi there!",
    "How are you doing?",
    "I'm doing great.",
    "That is good to hear",
    "Thank you.",
    "You're welcome.",
    "You are in a VR world.",
    "I'm your guider and also the god this world."
]

trainer = ListTrainer(chatbot)

trainer.train(conversation)

response = chatbot.get_response("Good morning!")
print(response)

response = chatbot.get_response("Who are you")
print(response)

response = chatbot.get_response("Where am I")
print(response)