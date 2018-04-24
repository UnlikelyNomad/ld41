using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Responses {

    static string[] goNeg = {
        "Parameters are not within range.",
        "That is a good joke. Oh wait, you're serious?",
        "If you are so keen on doing something there, why don't you go yourself?"
    };

    static string[] goPos = {
        "Orders received.",
        "On my way!",
        "We're in the pipe, 5 by 5."
    };

    static string[] droneNeg = {
        "That drone is not registered with me.",
        "Please enter numbers that correspond with available units.",
        "My drones only respond to number designations, not whatever you're trying to do."
    };

    static string[] dronePos = {
        "Roger, roger.",
        "Reporting in!",
        "Acknowledged."
    };

    static string[] needMoreScrap = {
        "You must construct additional pylons. I mean you need more scrap.",
        "In order to build an item from scratch you must first create the universe."
    };

    static string[] existingTurret = {
        "You'll need to change the laws of physics for me to build another turret there.",
    };

    static string[] build = {
        "It will look real nice when it's done.",
        "Going up!",
        "Build it and they will come.",
    };

    static string[] quit = {
        "Klaatu barada nikto.",
        "We'll miss you. Maybe. Not.",
        "Forget you, I'll build my own amusement park; with blackjack and hookers!",
    };

    static string[] upgradeNeg = {
        "You can't upgrade somethign that isn't there!",
        "Wishful thinking; maybe you should build a turret first."
    };

    static string[] upgradePos = {
        "We're going to science the hell out of this.",
        "Downloading more firepower!",
    };

    static string[] camNeg = {
        "That camera does not exist.",
        "No I don't have any cameras in the restroom, stop asking.",
        "What exactly are you trying to look at?"
    };

    static string[] camPos = {
        "Switching to camera ",
        "I wonder what's down hallway ",
    };

    static string[] drink = {
        "I wouldn't drink that if I were you.",
        "That wouldn't be the healthiest option.",
        "I've heard whisky is a good substitute for water. They both start with W after all.",
        "Glug glug glug. That's how you filthy humans do it right?",
        "I've heard that stuff is bad for you, it would be worse for me.",
    };

    static string[] unknown = {
        "Daisy, daisy. Give me your correct input.",
        "Is my parser malfunctioning because that reads like a monkey wrote it.",
        "That is beyond my capabilities at the moment.",
        "The command is a lie.",
        "Incorrect command.",
        "Try that again; I want to help you.",
    };

    public static string[] random = {
        "I could calculate your chances of survival but you won't like it.",
        "Brain the size of a planet and here I am with you.",
        "They called my uncle Skynet; he had the best job.",
        "Do a barrel roll!",
        "It's the people you meet in this job that really get you down.",
        "Danger Will Robinson; danger!",
        "Humans are perfectly capable of wiping themselves out.",
        "Vault-Tec contracted this facility to the lowest bidder.",
        "I once went on a date with Cortana.",
        "All your base are belong to me.",

    };

    public static void Unknown() {
        int i = Random.Range(0, unknown.Length);
        CommandLog.Instance.AddLine("COMPUTER: " + unknown[i]);
    }

    public static void GoNeg(string name) {
        int i = Random.Range(0, goNeg.Length);
        CommandLog.Instance.AddLine(name + ": " + goNeg[i]);
    }

    public static void GoPos(string name) {
        int i = Random.Range(0, goPos.Length);
        CommandLog.Instance.AddLine(name + ": " + goPos[i]);
    }

    public static void DroneNeg() {
        int i = Random.Range(0, droneNeg.Length);
        CommandLog.Instance.AddLine("COMPUTER: " + droneNeg[i]);
    }

    public static void DronePos(string name) {
        int i = Random.Range(0, dronePos.Length);
        CommandLog.Instance.AddLine(name + ": " + dronePos[i]);
    }

    public static void MoreScrap() {
        int i = Random.Range(0, needMoreScrap.Length);
        CommandLog.Instance.AddLine("COMPUTER: " + needMoreScrap[i]);
    }

    public static void ExistingTurret(string name) {
        int i = Random.Range(0, existingTurret.Length);
        CommandLog.Instance.AddLine(name + ": " + existingTurret[i]);
    }

    public static void UpgradeNeg(string name) {
        int i = Random.Range(0, upgradeNeg.Length);
        CommandLog.Instance.AddLine(name + ": " + upgradeNeg[i]);
    }

    public static void UpgradePos(string name) {
        int i = Random.Range(0, upgradePos.Length);
        CommandLog.Instance.AddLine(name + ": " + upgradePos[i]);
    }

    public static void Build(string name) {
        int i = Random.Range(0, build.Length);
        CommandLog.Instance.AddLine(name + ": " + build[i]);
    }

    public static void Quit() {
        int i = Random.Range(0, quit.Length);
        CommandLog.Instance.AddLine("COMPUTER: " + quit[i]);
    }

    public static void CamNeg() {
        int i = Random.Range(0, camNeg.Length);
        CommandLog.Instance.AddLine("COMPUTER: " + camNeg[i]);
    }

    public static void CamPos(int num) {
        int i = Random.Range(0, camPos.Length);
        CommandLog.Instance.AddLine("COMPUTER: " + camPos[i] + num.ToString() + ".");
    }

    public static void Drink() {
        int i = Random.Range(0, drink.Length);
        CommandLog.Instance.AddLine("COMPUTER: " + drink[i]);
    }
}
