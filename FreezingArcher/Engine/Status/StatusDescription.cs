//
//  StatusDescription.cs
//
//  Author:
//       Fin Christensen <christensen.fin@gmail.com>
//
//  Copyright (c) 2015 Fin Christensen
//
//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//
using System.Collections.Generic;

namespace FreezingArcher.Status
{
    /// <summary>
    /// This class provides the status codes with descriptions for logging.
    /// </summary>
    public static class StatusDescription
    {
        /// <summary>
        /// The global status code description table.
        /// </summary>
        public static readonly Dictionary<Status, string> Descriptions = new Dictionary<Status, string> ();

        static StatusDescription ()
        {
            Descriptions.Add (Status.Success, "A general code for an succeeded operation.");
	    Descriptions.Add (Status.Accepted, "A general code for an accepted communication operation.");
	    Descriptions.Add (Status.Found, "A resource was found.");
	    Descriptions.Add (Status.Loaded, "A resource was successfully loaded.");
	    Descriptions.Add (Status.PermissionGranted, "Permission was granted.");
	    Descriptions.Add (Status.Created, "An item was successfully created.");
	    Descriptions.Add (Status.Removed, "An item was successfully removed.");
	    Descriptions.Add (Status.MediaTypeSupported, "The media is type supported.");
	    Descriptions.Add (Status.EntityProcessed, "The entity was successfully processed.");
	    Descriptions.Add (Status.Computing, "General code for a working but not finished operation. " +
                "Until now no error was reported. Everything works as expected.");
	    Descriptions.Add (Status.Continue, "The sender is ready to continue processing with new data. " +
                "Everything worked as expected.");
	    Descriptions.Add (Status.Processing, "The sender is processing data but not ready to reveive new data. " +
                "Until now everything works as expected.");
	    Descriptions.Add (Status.Error, "A general status code indicating that something has gone wrong and the " +
                "operation has not finished with success.");
	    Descriptions.Add (Status.NotImplemented, "The requested operation is not implemented yet and could not " +
                "be executed.");
	    Descriptions.Add (Status.BadArgument, "The given argument is invalid.");
	    Descriptions.Add (Status.BadData, "The given data is invalid.");
	    Descriptions.Add (Status.DataNotAvailable, "The requested data is not available.");
	    Descriptions.Add (Status.NotEnoughData, "There is not enough data given to proceed with the operation.");
	    Descriptions.Add (Status.UnexpectedData, "The given data is well formed but does not fit in the context.");
	    Descriptions.Add (Status.Caching, "Error while caching data.");
	    Descriptions.Add (Status.DataCachedForTooLong, "Data was cached for too long.");
	    Descriptions.Add (Status.DataNotCachedLongEnough, "Data was not cached long enough.");
	    Descriptions.Add (Status.DataNotCached, "Data was not cached at all.");
	    Descriptions.Add (Status.WhyWasThisCached, "Why was this data cached. It should not be cached.");
	    Descriptions.Add (Status.OutOfCash, "See OutOfMemory.");
	    Descriptions.Add (Status.BadRequest, "The communication request is invalid.");
	    Descriptions.Add (Status.Aborted, "Communication was aborted because of an error.");
	    Descriptions.Add (Status.Rejected, "The communication request was rejected.");
	    Descriptions.Add (Status.TimedOut, "The communication request timed out.");
	    Descriptions.Add (Status.TooManyRequests, "The communication manager is not able to process all requests.");
	    Descriptions.Add (Status.LoopDetected, "A communication loop has been detected and communication has " +
                "been canceled.");
	    Descriptions.Add (Status.PermissionDenied, "Access to this resource has been denied.");
	    Descriptions.Add (Status.NotFound, "The requested resource was not found.");
	    Descriptions.Add (Status.NotAvailable, "The requested resource is not available.");
	    Descriptions.Add (Status.OutOfMemory, "System is out of memory.");
	    Descriptions.Add (Status.FailedToCreate, "Failed to create item.");
	    Descriptions.Add (Status.FailedToRemove, "Failed to remove item.");
	    Descriptions.Add (Status.NoSpaceLeftOnDevice, "Could not finish operation as there is no space left on " +
                "device.");
	    Descriptions.Add (Status.ModifiedFromOutside, "The specified resource was modified from outside this " +
                "application and is causing an inconsistent data set.");
	    Descriptions.Add (Status.Locked, "The resource is locked and can not be modified.");
	    Descriptions.Add (Status.UnsupportedMediaType, "The requested resource is of an unsupported media type.");
	    Descriptions.Add (Status.UnprocessableEntity, "The requested entity is not processable.");
	    Descriptions.Add (Status.Meh, "General debug/testing error.");
	    Descriptions.Add (Status.Emacs, "Only Emacs will be able to handle this operation.");
	    Descriptions.Add (Status.Vim, "Only Vim will be able to handle this operation.");
	    Descriptions.Add (Status.VisualStudio, "Only Visual Studio will be able to handle this operation.");
	    Descriptions.Add (Status.WellWithSomeLispAndTimeYouMayAchieveThisWithEmacs, "Well with some lisp and " +
                "time you may achieve this with emacs.");
	    Descriptions.Add (Status.ThereIsAnEmacsMacroForIt, "There is an emacs macro for it, I just can't " +
                "remember it.");
	    Descriptions.Add (Status.Explosion, "System exploded.");
	    Descriptions.Add (Status.ClimateChangeDrivenCatastrophicWeatherEvent, "A climate change driven " +
                "catastrophic weather event is preventing the developer from implementing this operation as expected.");
	    Descriptions.Add (Status.ZombieApocalypse, "A zombie apocalypse is preventing the developer from " +
                "implementing this operation as expected.");
	    Descriptions.Add (Status.Impossible, "An impossible edge case has been reached.");
	    Descriptions.Add (Status.KnownUnknowns, "We know the result is unknown.");
	    Descriptions.Add (Status.UnknownUnknowns, "We do not know if the result is unknown.");
	    Descriptions.Add (Status.Tricky, "This operation is a bit tricky and needs some sanitizing.");
	    Descriptions.Add (Status.UnreachableLineReached, "An unreachable code has been reached. Stop using " +
                "reflections!");
	    Descriptions.Add (Status.ThisWorksOnMyMachine, "Well this code works fine on my machine.");
	    Descriptions.Add (Status.ItsNotABugItsAFeature, "The unexpected results are planed and shall be " +
                "interpreted as a feature and not as a bug.");
	    Descriptions.Add (Status.DamnUnicode, "Unicode smashed my dreams.");
	    Descriptions.Add (Status.DamnDeadlocks, "Deadlocks are driving me crazy.");
	    Descriptions.Add (Status.DamnDeferreds, "We defenitely need a manager for this deferred operations.");
	    Descriptions.Add (Status.DamnRaceConditions, "Race conditions are quite difficult to debug so do not " +
                "expect this to be fixed in the near future.");
	    Descriptions.Add (Status.DamnThreading, "We encountered some threading problems. This may be fixed with " +
                "a try-catch-retry...");
	    Descriptions.Add (Status.DamnIntelGraphics, "Well intel drivers are ... quite special.");
	    Descriptions.Add (Status.DamnNVidiaGraphics, "NVidia graphics seem to ignore this while other drivers " +
                "are crashing.");
	    Descriptions.Add (Status.DamnAMDGraphics, "The behaviour of AMD graphics is not compatible across " +
                "different versions. Try using another driver version.");
	    Descriptions.Add (Status.DamnRegex, "I thought I knew regular expressions...");
	    Descriptions.Add (Status.IThoughtIKnewReflection, "I thought I knew reflection...");
	    Descriptions.Add (Status.DeveloperHadHangover, "The developer had a hangover.");
	    Descriptions.Add (Status.DeveloperWasStoned, "The developer was stoned while writing this code.");
	    Descriptions.Add (Status.DeveloperWasUnderCaffeinated, "The developer was under caffeinated while " +
                "writing this code.");
	    Descriptions.Add (Status.DeveloperWasOverCaffeinated, "The developer was over caffeinated while writing " +
                "this code.");
	    Descriptions.Add (Status.DeveloperWasSober, "The developer was sober while writing this code.");
	    Descriptions.Add (Status.DeveloperWasDrunk, "The developer was drunk while writing this code.");
	    Descriptions.Add (Status.DeveloperAccidentallyTookSleepingPillsInsteadOfMigrainePills, "The developer " +
                "accidentally took sleeping pills instead of migraine pills.");
	    Descriptions.Add (Status.AKittenDies, "A kitten may die if this happens.");
	    Descriptions.Add (Status.MissedBallmerPeak, "The developer missed the ballmer peak.");
	    Descriptions.Add (Status.ConfoundedByPonies, "The developer was confounded by ponies.");
	    Descriptions.Add (Status.ChuckNorrisGotYou, "Chuck Norris got you.");
	    Descriptions.Add (Status.ProjectOwnerNotResponding, "The project owner is not responding. Leaving this " +
                "code broken...");
	    Descriptions.Add (Status.ManagementThingy, "Such foo...");
	    Descriptions.Add (Status.DearUserRTFM, "Dear user, would you be so kind to just READ THE FUCKING " +
                "MANUAL!!!1111eleven");
	    Descriptions.Add (Status.WhoNeedsDocs, "Who needs a manual?");
	    Descriptions.Add (Status.HeyItsMeTheGarbageCollector, "The garbage collector is working against the " +
                "developer.");
	    Descriptions.Add (Status.WhoNeedsIntegrationTests, "Integration tests are for noobs.");
	    Descriptions.Add (Status.YUNoWriteIntegrationTests, "Y U NO write integration tests??");
        }
    }
}
