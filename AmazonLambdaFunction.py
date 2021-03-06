# -*- coding: utf-8 -*-
import os
import json
import logging
import urllib3

_debug = True

_logger = logging.getLogger('Otto-Intents')
_logger.setLevel(logging.DEBUG if _debug else logging.INFO)


def lambda_handler(event, context):
    """Handle incoming Alexa directive."""

    _logger.debug('Event: %s', event)
    
    #Fill in your HomeAssistant webhook url here
    base_url = 'https://HASSURL/api/webhook/WEBHOOKID'
    
    
    #Can set to False to verify ssl
    not_verify_ssl = True;
    
    #Intent slot
    try:
        intentslot = event['request']['intent']['slots']['intentslot']['resolutions']['resolutionsPerAuthority'][0]['values'][0]['value']['name']
    except:
        intentslot = "null"
    
    #Utterance slot
    try:
        utteranceslot = event['request']['intent']['slots']['utteranceslot']['value']
    except:
        utteranceslot = "null"
        
    #Number slot
    try:
        numberslot = event['request']['intent']['slots']['numberslot']['value']
    except:
        numberslot = "null"
    
    #connector slot
    try:
        connectorslot = event['request']['intent']['slots']['connectorslot']['value']
    except:
        connectorslot = "null"
        
    #Duration slot
    try:
        timeslot = event['request']['intent']['slots']['timeslot']['value']
    except:
        timeslot = "null"
    
    jrequest = '{"intent":"' + intentslot + '","utterance":"'+utteranceslot + '","number":"' + numberslot + '","connector":"' + connectorslot + '","time":"' + timeslot +'"}'
    

    #jrequest = json.dumps(event['request'], separators=(',', ':')).encode('utf-8')

    http = urllib3.PoolManager(
        cert_reqs='CERT_REQUIRED' if not_verify_ssl else 'CERT_NONE',
        timeout=urllib3.Timeout(connect=5.0, read=10.0)
    )
    
    response = http.request(
        'POST',
        base_url,
        headers={
            'Content-Type': 'application/json'
        },
        body = json.dumps(jrequest, separators=(',', ':')).encode('utf-8')
    )
    if response.status >= 400:
        return '{"response":{"outputSpeech":{"type":"SSML","ssml":"<speak>There was a problem reaching the server<\/speak>"},"reprompt":{"outputSpeech":{"type":"SSML","ssml":"<speak><\/speak>"}},"shouldEndSession":true},"version":"1.0","sessionAttributes":{}}'
    return '{"response":{"outputSpeech":{"type":"SSML","ssml":"<speak>OK<\/speak>"},"reprompt":{"outputSpeech":{"type":"SSML","ssml":"<speak><\/speak>"}},"shouldEndSession":true},"version":"1.0","sessionAttributes":{}}'
