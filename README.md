<!-- -*- mode: markdown; fill-column: 8192 -*- -->

Notepad++ Plugin for Swift Messages
======================
NppSWIFT is a plugin for [Notepad++](https://notepad-plus-plus.org/) which will convert the machine readable text into something slightly more friendly for humans.

[![plugin-screenshot-example](https://github.com/joshuabragge/NppSWIFT/blob/master/Examples/Images/plugin-screenshot-example.JPG)

### Before MT950 Message Conversion

[![before-screenshot-conversion](https://github.com/joshuabragge/NppSWIFT/blob/master/Examples/Images/message-before-screenshot-example.JPG)

### After MT950 Message Conversion

[![after-screenshot-conversion](https://github.com/joshuabragge/NppSWIFT/blob/master/Examples/Images/message-after-screenshot-example.JPG)

## Table of Contents
* [Overview](#notepad-plugin-for-swift-messages)
* [Getting Started](#getting-started)
* [Supported Messages](#supported-messages)
* [Examples](#examples)

## Getting Started

Do one of the following depending on your version of Notepad++

* Download the x86 `SWIFTPlugin.dll` and place it in the `C:\program files (x86)\notepad++\plugins\` folder.
* Download the x64 `SWIFTPlugin.dll` and place it in the `C:\program files\notepad++\plugins\` folder.

## Supported Messages
* MT950

## Examples 

With the message(s) as the active tab in Notepad++, click the following:
1. Plugins
2. SWIFT
3. MT950

Before processing:
`{1:F01SAESVAV0AXXX0466020565}{2:O9501552070522LRLRXXXX4A0700005915090705221752N}{3:{108:MT950 007 OF 030}}
{4::20:02711:25:210057665066:28C:00196:60F:C000103HKD672,:61:000103D642,NRTI9999CHECK NO. 57650:62F:C000103HKD30,-}
{5:{CHK:22ED16B18106}{TNG:}}`

After processing:
`{1:F01SAESVAV0AXXX0466020565}
{2:O9501552070522LRLRXXXX4A0700005915090705221752N}
transaction_reference_number:02711
account_identification_statement:210057665066
statement_sequence_number:00196
opening_balance:C000103HKD672,
	debit_or_credit:C
	date:000103
	currency:HKD
	amount:672,
statement_line:000103D642
	debit_or_credit:D
	value_date:000103
	entry_date:
	funds_code:
	amount:642
	transaction_type:
	reference_account_owner:
	refernece_institution:
	supplementary_details:
closing_balance:C000103HKD30,
	debit_or_credit:C
	date:000103
	currency:HKD
	amount:30,
-}
{5:{CHK:22ED16B18106}{TNG:}}`