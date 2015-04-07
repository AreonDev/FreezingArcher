//
//  StatusEnum.cs
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

namespace FreezingArcher.Core
{
    /// <summary>
    /// Enumeration for status codes. 0-99 are success codes, 100-199 are codes for running operations,
    /// 200-299 are error codes, 300+ are debug and testing codes.
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// A general code for an succeeded operation.
        /// </summary>
        Success = 0,

        /// <summary>
        /// A general code for an accepted communication operation.
        /// </summary>
        Accepted = 10,

        /// <summary>
        /// A resource was found.
        /// </summary>
        Found = 20,
        /// <summary>
        /// A resource was successfully loaded.
        /// </summary>
        Loaded,
        /// <summary>
        /// Permission was granted.
        /// </summary>
        PermissionGranted,
        /// <summary>
        /// An item was successfully created.
        /// </summary>
        Created,
        /// <summary>
        /// An item was successfully removed.
        /// </summary>
        Removed,
        /// <summary>
        /// The media is type supported.
        /// </summary>
        MediaTypeSupported,
        /// <summary>
        /// The entity was successfully processed.
        /// </summary>
        EntityProcessed,

        /// <summary>
        /// General code for a working but not finished operation. Until now no error was reported. Everything works as
        /// expected.
        /// </summary>
        Computing = 100,

        /// <summary>
        /// The sender is ready to continue processing with new data. Everything worked as expected.
        /// </summary>
        Continue = 110,
        /// <summary>
        /// The sender is processing data but not ready to reveive new data. Until now everything works as expected.
        /// </summary>
        Processing,

        /// <summary>
        /// A general status code indicating that something has gone wrong and the operation has not finished with
        /// success.
        /// </summary>
        Error = 200,
        /// <summary>
        /// The requested operation is not implemented yet and could not be executed.
        /// </summary>
        NotImplemented,

        /// <summary>
        /// The given argument is invalid.
        /// </summary>
        BadArgument,
        /// <summary>
        /// The given data is invalid.
        /// </summary>
        BadData,
        /// <summary>
        /// The requested data is not available.
        /// </summary>
        DataNotAvailable,
        /// <summary>
        /// There is not enough data given to proceed with the operation.
        /// </summary>
        NotEnoughData,
        /// <summary>
        /// The given data is well formed but does not fit in the context.
        /// </summary>
        UnexpectedData,

        /// <summary>
        /// Error while caching data.
        /// </summary>
        Caching = 210,
        /// <summary>
        /// Data was cached for too long.
        /// </summary>
        DataCachedForTooLong,
        /// <summary>
        /// Data was not cached long enough.
        /// </summary>
        DataNotCachedLongEnough,
        /// <summary>
        /// Data was not cached at all.
        /// </summary>
        DataNotCached,
        /// <summary>
        /// Why was this data cached. It should not be cached.
        /// </summary>
        WhyWasThisCached,
        /// <summary>
        /// See OutOfMemory.
        /// </summary>
        OutOfCash,

        /// <summary>
        /// The communication request is invalid.
        /// </summary>
        BadRequest = 220,
        /// <summary>
        /// Communication was aborted because of an error.
        /// </summary>
        Aborted,
        /// <summary>
        /// The communication request was rejected.
        /// </summary>
        Rejected,
        /// <summary>
        /// The communication request timed out.
        /// </summary>
        TimedOut,
        /// <summary>
        /// The communication manager is not able to process all requests.
        /// </summary>
        TooManyRequests,
        /// <summary>
        /// A communication loop has been detected and communication has been canceled.
        /// </summary>
        LoopDetected,

        /// <summary>
        /// Access to this resource has been denied.
        /// </summary>
        PermissionDenied = 230,
        /// <summary>
        /// The requested resource was not found.
        /// </summary>
        NotFound,
        /// <summary>
        /// The requested resource is not available.
        /// </summary>
        NotAvailable,
        /// <summary>
        /// System is out of memory.
        /// </summary>
        OutOfMemory,
        /// <summary>
        /// Failed to create item.
        /// </summary>
        FailedToCreate,
        /// <summary>
        /// Failed to remove item.
        /// </summary>
        FailedToRemove,
        /// <summary>
        /// Could not finish operation as there is no space left on device.
        /// </summary>
        NoSpaceLeftOnDevice,
        /// <summary>
        /// The specified resource was modified from outside this application and is causing an inconsistent data set.
        /// </summary>
        ModifiedFromOutside,
        /// <summary>
        /// The resource is locked and can not be modified.
        /// </summary>
        Locked,
        /// <summary>
        /// The requested resource is of an unsupported media type.
        /// </summary>
        UnsupportedMediaType,
        /// <summary>
        /// The requested entity is not processable.
        /// </summary>
        UnprocessableEntity,

        /// <summary>
        /// General debug/testing error.
        /// </summary>
        Meh = 300,
        /// <summary>
        /// Only Emacs will be able to handle this operation.
        /// </summary>
        Emacs,
        /// <summary>
        /// Only Vim will be able to handle this operation.
        /// </summary>
        Vim,
        /// <summary>
        /// Only Visual Studio will be able to handle this operation.
        /// </summary>
        VisualStudio,
        /// <summary>
        /// Well with some lisp and time you may achieve this with emacs.
        /// </summary>
        WellWithSomeLispAndTimeYouMayAchieveThisWithEmacs,
        /// <summary>
        /// There is an emacs macro for it, I just can't remember it.
        /// </summary>
        ThereIsAnEmacsMacroForIt,
        /// <summary>
        /// System exploded.
        /// </summary>
        Explosion,
        /// <summary>
        /// A climate change driven catastrophic weather event is preventing the developer from implementing this
        /// operation as expected.
        /// </summary>
        ClimateChangeDrivenCatastrophicWeatherEvent,
        /// <summary>
        /// A zombie apocalypse is preventing the developer from implementing this operation as expected.
        /// </summary>
        ZombieApocalypse,

        /// <summary>
        /// An impossible edge case has been reached.
        /// </summary>
        Impossible = 310,
        /// <summary>
        /// We know the result is unknown.
        /// </summary>
        KnownUnknowns,
        /// <summary>
        /// We do not know if the result is unknown.
        /// </summary>
        UnknownUnknowns,
        /// <summary>
        /// This operation is a bit tricky and needs some sanitizing.
        /// </summary>
        Tricky,
        /// <summary>
        /// An unreachable code has been reached. Stop using reflections!
        /// </summary>
        UnreachableLineReached,
        /// <summary>
        /// Well this code works fine on my machine.
        /// </summary>
        ThisWorksOnMyMachine,
        /// <summary>
        /// The unexpected results are planed and shall be interpreted as a feature and not as a bug.
        /// </summary>
        ItsNotABugItsAFeature,

        /// <summary>
        /// Unicode smashed my dreams.
        /// </summary>
        DamnUnicode = 320,
        /// <summary>
        /// Deadlocks are driving me crazy.
        /// </summary>
        DamnDeadlocks,
        /// <summary>
        /// We defenitely need a manager for this deferred operations.
        /// </summary>
        DamnDeferreds,
        /// <summary>
        /// Race conditions are quite difficult to debug so do not expect this to be fixed in the near future.
        /// </summary>
        DamnRaceConditions,
        /// <summary>
        /// We encountered some threading problems. This may be fixed with a try-catch-retry...
        /// </summary>
        DamnThreading,
        /// <summary>
        /// Well intel drivers are ... quite special.
        /// </summary>
        DamnIntelGraphics,
        /// <summary>
        /// NVidia graphics seem to ignore this while other drivers are crashing.
        /// </summary>
        DamnNVidiaGraphics,
        /// <summary>
        /// The behaviour of AMD graphics is not compatible across different versions. Try using another driver version.
        /// </summary>
        DamnAMDGraphics,
        /// <summary>
        /// I thought I knew regular expressions...
        /// </summary>
        DamnRegex,
        /// <summary>
        /// I thought I knew reflection...
        /// </summary>
        IThoughtIKnewReflection,

        /// <summary>
        /// The developer had a hangover.
        /// </summary>
        DeveloperHadHangover = 330,
        /// <summary>
        /// The developer was stoned while writing this code.
        /// </summary>
        DeveloperWasStoned,
        /// <summary>
        /// The developer was under caffeinated while writing this code.
        /// </summary>
        DeveloperWasUnderCaffeinated,
        /// <summary>
        /// The developer was over caffeinated while writing this code.
        /// </summary>
        DeveloperWasOverCaffeinated,
        /// <summary>
        /// The developer was sober while writing this code.
        /// </summary>
        DeveloperWasSober,
        /// <summary>
        /// The developer was drunk while writing this code.
        /// </summary>
        DeveloperWasDrunk,
        /// <summary>
        /// The developer accidentally took sleeping pills instead of migraine pills.
        /// </summary>
        DeveloperAccidentallyTookSleepingPillsInsteadOfMigrainePills,

        /// <summary>
        /// A kitten may die if this happens.
        /// </summary>
        AKittenDies = 340,
        /// <summary>
        /// The developer missed the ballmer peak.
        /// </summary>
        MissedBallmerPeak,
        /// <summary>
        /// The developer was confounded by ponies.
        /// </summary>
        ConfoundedByPonies,
        /// <summary>
        /// Chuck Norris got you.
        /// </summary>
        ChuckNorrisGotYou,
        /// <summary>
        /// The project owner is not responding. Leaving this code broken...
        /// </summary>
        ProjectOwnerNotResponding,
        /// <summary>
        /// Such foo...
        /// </summary>
        ManagementThingy,
        /// <summary>
        /// Dear user, would you be so kind to just READ THE FUCKING MANUAL!!!1111eleven
        /// </summary>
        DearUserRTFM,
        /// <summary>
        /// Who needs a manual?
        /// </summary>
        WhoNeedsDocs,
        /// <summary>
        /// The garbage collector is working against the developer.
        /// </summary>
        HeyItsMeTheGarbageCollector,
        /// <summary>
        /// Integration tests are for noobs.
        /// </summary>
        WhoNeedsIntegrationTests,
        /// <summary>
        /// Y U NO write integration tests??
        /// </summary>
        YUNoWriteIntegrationTests,
    }
}
