﻿using System.Collections.Generic;
using System.Concurrency;
using System.Linq;
using System.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BarcodeScannerRx.Tests
{
    [TestClass]
    public class BarcodeReaderOperatorTests : RxTest
    {
        [TestMethod]
        public void InputHasStartAndEndMarkersWithValuesInBetween()
        {
            var scheduler = new TestScheduler();
            var inputSequence = scheduler.CreateHotObservable(
                OnNextForAll(250, "^hello$"),
                OnCompleted<char>(300)
                );

            var results = scheduler.Run(() => inputSequence.ToBarcodeReadings());

            results.AssertEqual(EnumerableEx.Concat(
                OnNext(250, "hello"),
                OnCompleted<string>(300)
                ));

            inputSequence.Subscriptions.AssertEqual(
                Subscribe(200, 300)
                );
        }

        [TestMethod]
        public void InputHasMultipleStartsBeforeEndMarker()
        {
            var scheduler = new TestScheduler();
            var inputSequence = scheduler.CreateHotObservable(
                OnNextForAll(250, "^^blah^world$")
                );

            var results = scheduler.Run(() => inputSequence.ToBarcodeReadings());

            results.AssertEqual(EnumerableEx.Concat(
                OnNext(250, "world")
                ));

            inputSequence.Subscriptions.AssertEqual(
                Subscribe(200)
                );
        }

        [TestMethod]
        public void InputHasNoValidBarcodeSequence()
        {
            var scheduler = new TestScheduler();
            var inputSequence = scheduler.CreateHotObservable(
                OnNextForAll(250, "blah some text blah yay!"),
                OnCompleted<char>(500)
                );

            var results = scheduler.Run(() => inputSequence.ToBarcodeReadings());          

            results.AssertEqual(
                OnCompleted<string>(500)
                );

            inputSequence.Subscriptions.AssertEqual(
                Subscribe(200, 500)
            );
        }

        [TestMethod]
        public void InputSequenceHasStartButNoEndMarker()
        {
            var scheduler = new TestScheduler();
            var inputSequence = scheduler.CreateHotObservable(
                OnNextForAll(250, "blah^helloworld"),
                OnCompleted<char>(300)
                );

            var results = scheduler.Run(() => inputSequence.ToBarcodeReadings());

            results.AssertEqual(
                OnCompleted<string>(300)
                );

            inputSequence.Subscriptions.AssertEqual(
                Subscribe(200, 300)
            );
        }

        [TestMethod]
        public void InputSequenceHasEndButNoStartMarker()
        {
            var scheduler = new TestScheduler();
            var inputSequence = scheduler.CreateHotObservable(
                OnNextForAll(250, "HowAreYouToday?$xyz"),
                OnCompleted<char>(300)
                );

            var results = scheduler.Run(() => inputSequence.ToBarcodeReadings());

            results.AssertEqual(
                OnCompleted<string>(300)
                );

            inputSequence.Subscriptions.AssertEqual(
                Subscribe(200, 300)
                );
        }

        [TestMethod]
        public void InputHasMultipleEndMarkersBeforeTheSecondSequenceBegins()
        {
            var scheduler = new TestScheduler();
            var inputSequence = scheduler.CreateHotObservable(
                OnNextForAll(250, "someText^abcd$efg$1337^rofl$lmnop"),
                OnCompleted<char>(300)
                );

            var results = scheduler.Run(() => inputSequence.ToBarcodeReadings());

            results.AssertEqual(EnumerableEx.Concat(
                OnNext(250, "abcd"),
                OnNext(250, "rofl"),
                OnCompleted<string>(300)
                ));

            inputSequence.Subscriptions.AssertEqual(
                Subscribe(200,300)
                );
        }
        
        [TestMethod]
        public void InputHasNoCharactersBetweenStartAndEndMarkers()
        {
            var scheduler = new TestScheduler();
            var inputSequence = scheduler.CreateHotObservable(
                OnNextForAll(250, "abcd^$efg"),
                OnCompleted<char>(300)
                );

            var results = scheduler.Run(() => inputSequence.ToBarcodeReadings());

            results.AssertEqual(EnumerableEx.Concat(
                OnNext(250, string.Empty),
                OnCompleted<string>(300)
                ));

            inputSequence.Subscriptions.AssertEqual(
                Subscribe(200,300)
                );
        }

        [TestMethod]
        public void InputHasMultipleBarcodeSequencesAndFalseStartsAndEnds()
        {
            var scheduler = new TestScheduler();
            var inputSequence = scheduler.CreateHotObservable(
                OnNextForAll(250, "a$bc^^123^653"),
                OnNextForAll(260, "4345$$^^"),
                OnNextForAll(270, "$^^^345345"),
                OnNextForAll(280, "^23423$def^bah"),
                OnCompleted<char>(300)
                );

            var results = scheduler.Run(() => inputSequence.ToBarcodeReadings());

            results.AssertEqual(EnumerableEx.Concat(
                OnNext(260, "6534345"),
                OnNext(270, string.Empty),
                OnNext(280, "23423"),
                OnCompleted<string>(300)
                ));

            inputSequence.Subscriptions.AssertEqual(
                Subscribe(200,300)
                );
        }

        [TestMethod]
        public void InputHasADelayBetweenCharactersOfLongerThanFiveSecondsDuringSequence()
        {
            var scheduler = new TestScheduler();
            var inputSequence = scheduler.CreateHotObservable(
                OnNextForAll(1.0.Seconds(), "a^hello"),
                OnNextForAll(6.1.Seconds(), "world$"),
                OnNextForAll(7.0.Seconds(), "^Rx$"),
                OnCompleted<char>(7.5.Seconds())
                );

            var results = scheduler.Run(() => inputSequence.ToBarcodeReadings(scheduler), 
                0.0.Seconds().Ticks, 
                0.5.Seconds().Ticks, 
                10.Seconds().Ticks
                );

            results.AssertEqual(EnumerableEx.Concat(
                OnNext(7.Seconds(), "Rx"),
                OnCompleted<string>(7.5.Seconds())
                ));

            inputSequence.Subscriptions.AssertEqual(
                Subscribe(0.5.Seconds(), 7.5.Seconds())
                );
        }

        [TestMethod]
        public void InputHasADelayBetweenCharactersOfExactlyFiveSecondsDuringSequence()
        {
            var scheduler = new TestScheduler();
            var inputSequence = scheduler.CreateHotObservable(
                OnNextForAll(1.Seconds(), "a^hello"),
                OnNextForAll(6.Seconds(), "world$"),
                OnNextForAll(7.Seconds(), "^Rx$"),
                OnCompleted<char>(7.5.Seconds())
                );

            var results = scheduler.Run(() => inputSequence.ToBarcodeReadings(scheduler), 
                0.0.Seconds().Ticks, 
                0.5.Seconds().Ticks, 
                10.Seconds().Ticks
                );

            results.AssertEqual(EnumerableEx.Concat(
                OnNext(6.Seconds(), "helloworld"),
                OnNext(7.Seconds(), "Rx"),
                OnCompleted<string>(7.5.Seconds())
                ));

            inputSequence.Subscriptions.AssertEqual(
                Subscribe(0.5.Seconds(), 7.5.Seconds())
                );            
        }

        [TestMethod]
        public void InputHasADelayBetweenEachCharacterOfExaclyFiveSecondsDuringSequence()
        {
            var scheduler = new TestScheduler();
            var inputSequence = scheduler.CreateHotObservable(
                OnNextForAll(1.Seconds(), "a^helloworld$bc").TimeBetweenEach(5.Seconds()),
                OnCompleted<char>(100.Seconds())
                );

            var results = scheduler.Run(() => inputSequence.ToBarcodeReadings(scheduler),
                0.0.Seconds().Ticks,
                0.5.Seconds().Ticks,
                100.Seconds().Ticks
                );

            results.AssertEqual(EnumerableEx.Concat(
                OnNext(61.Seconds(), "helloworld"),
                OnCompleted<string>(100.Seconds())
                ));

            inputSequence.Subscriptions.AssertEqual(
                Subscribe(0.5.Seconds(), 100.Seconds())
                );            
        }

        [TestMethod]
        public void InputHasADelayBetweenEachCharacterOfJustOverFiveSecondsDuringSequence()
        {
            var scheduler = new TestScheduler();
            var inputSequence = scheduler.CreateHotObservable(
                OnNextForAll(1.Seconds(), "a^helloworld$bc").TimeBetweenEach(5.01.Seconds()),
                OnCompleted<char>(100.Seconds())
                );

            var results = scheduler.Run(() => inputSequence.ToBarcodeReadings(scheduler),
                0.0.Seconds().Ticks,
                0.5.Seconds().Ticks,
                100.Seconds().Ticks
                );

            results.AssertEqual(EnumerableEx.Concat(
                OnCompleted<string>(100.Seconds())
                ));

            inputSequence.Subscriptions.AssertEqual(
                Subscribe(0.5.Seconds(), 100.Seconds())
                );
        }
    }
}