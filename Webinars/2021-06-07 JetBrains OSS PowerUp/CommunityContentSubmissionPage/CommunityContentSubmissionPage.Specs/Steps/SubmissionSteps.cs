﻿using System;
using System.Collections.Generic;
using System.Linq;
using Boa.Constrictor.Screenplay;
using Boa.Constrictor.WebDriver;
using CommunityContentSubmissionPage.Specs.Drivers;
using CommunityContentSubmissionPage.Specs.Interactions;
using CommunityContentSubmissionPage.Specs.Pages;
using CommunityContentSubmissionPage.Test.Common;
using FluentAssertions;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace CommunityContentSubmissionPage.Specs.Steps
{
    [Binding]
    public class SubmissionSteps
    {
        private readonly Actor _actor;
        private readonly SubmissionDriver _submissionDriver;

        public SubmissionSteps(SubmissionDriver submissionDriver, Actor actor)
        {
            _submissionDriver = submissionDriver;
            _actor = actor;
        }

        [Then(@"it is possible to enter a '(.*)' with label '(.*)'")]
        public void ThenItIsPossibleToEnterAWithLabel(string inputType, string expectedLabel)
        {
            IWebLocator inputFieldLocator;
            IWebLocator labelLocator;

            switch (inputType.ToUpper())
            {
                case "URL":
                    inputFieldLocator = SubmissionPage.UrlInputField;
                    labelLocator = SubmissionPage.UrlLabel;
                    break;
                case "TYPE":
                    inputFieldLocator = SubmissionPage.TypeSelect;
                    labelLocator = SubmissionPage.TypeLabel;
                    break;
                case "EMAIL":
                    inputFieldLocator = SubmissionPage.EmailInputField;
                    labelLocator = SubmissionPage.EmailLabel;
                    break;
                case "DESCRIPTION":
                    inputFieldLocator = SubmissionPage.DescriptionInputField;
                    labelLocator = SubmissionPage.DescriptionLabel;
                    break;
                case "NAME":
                    inputFieldLocator = SubmissionPage.NameField;
                    labelLocator = SubmissionPage.NameLabel;
                    break;
                case "PRIVACY POLICY":
                    inputFieldLocator = SubmissionPage.PrivacyPolicy;
                    labelLocator = SubmissionPage.PrivacyPolicyLabel;
                    break;
                default:
                    throw new NotImplementedException();
            }

            _actor.AttemptsTo(Wait.Until(Appearance.Of(inputFieldLocator), IsEqualTo.True()));
            _actor.AskingFor(Text.Of(labelLocator)).Should().Be(expectedLabel);
        }

        [Given(@"the filled out submission entry form")]
        public void GivenTheFilledOutSubmissionEntryForm(Table table)
        {
            var rows = table.CreateSet<SubmissionEntryFormRowObject>();

            _actor.AttemptsTo(FillOutSubmissionForm.With(rows));
        }

        [When(@"the form is submitted")]
        [When(@"the submission entry form is submitted")]
        public void WhenTheSubmissionEntryFormIsSubmitted()
        {
            _actor.AttemptsTo(Click.On(SubmissionPage.SubmitButton));
        }

        [Then(@"there is one submission entry stored")]
        public void ThenThereIsOneSubmissionEntryStored()
        {
            _submissionDriver.AssertOneSubmissionEntryExists();
        }

        [Then(@"there is '(.*)' submission entry stored")]
        public void ThenThereIsSubmissionEntryStored(int expectedCountOfStoredEntries)
        {
            _submissionDriver.AssertNumberOfEntriesStored(expectedCountOfStoredEntries);
        }

        [Then(@"the submitting of data was possible")]
        public void ThenTheSubmittingOfDataWasPossible()
        {
            _actor.AsksFor(CurrentUrl.FromBrowser()).Should()
                .EndWith("Success", "because the success page should be displayed");
        }

        [Then(@"the submitting of data was not possible")]
        public void ThenTheSubmittingOfDataWasNotPossible()
        {
            _actor.AsksFor(CurrentUrl.FromBrowser()).Should()
                .NotEndWith("Success", "the input form page should be displayed again");
        }


        [Then(@"you can choose from the following types:")]
        public void ThenYouCanChooseFromTheFollowingTypes(Table table)
        {
            var expectedTypenameEntries = table.CreateSet<TypenameEntry>();
            var actualTypes = _actor.AsksFor(SelectOptionsAvailable.For(SubmissionPage.TypeSelect))
                .Select(i => new TypenameEntry(i));

            actualTypes.Should().BeEquivalentTo(expectedTypenameEntries);
        }

        [Given(@"the submission entry form is filled")]
        public void GivenTheSubmissionEntryFormIsFilled()
        {
            var submissionEntryFormRowObjects = new List<SubmissionEntryFormRowObject>
            {
                new("Url", "https://example.org"),
                new("Type", "Blog Posts"),
                new("Email", "someone@example.org"),
                new("Description", "something really cool"),
                new("Name", "Jane Doe"),
                new("Privacy Policy", "true")
            };

            _actor.AttemptsTo(FillOutSubmissionForm.With(submissionEntryFormRowObjects));
        }

        [Given(@"the privacy policy is not accepted")]
        public void GivenThePrivacyPolicyIsNotAccepted()
        {
            var privacyPolicyIsChecked = _actor.AskingFor(SelectedState.Of(SubmissionPage.PrivacyPolicy));
            if (privacyPolicyIsChecked) _actor.AttemptsTo(Click.On(SubmissionPage.PrivacyPolicy));
        }

        [Given(@"the privacy policy is accepted")]
        public void GivenThePrivacyPolicyIsAccepted()
        {
            _actor.AttemptsTo(Click.On(SubmissionPage.PrivacyPolicy));
        }

        [When(@"the form is reset")]
        public void WhenTheFormIsReset()
        {
            _actor.AttemptsTo(Click.On(SubmissionPage.CancelButton));
        }

        [Then(@"every input is set to its default value")]
        public void ThenEveryInputIsSetToItsDefaultValues()
        {
            _actor.AsksFor(Text.Of(SubmissionPage.UrlInputField)).Should().BeEmpty();
            _actor.AsksFor(SelectedOptionText.Of(SubmissionPage.TypeSelect)).Should().Be("Blog Posts");
            _actor.AsksFor(Text.Of(SubmissionPage.EmailInputField)).Should().BeEmpty();
            _actor.AsksFor(Text.Of(SubmissionPage.DescriptionInputField)).Should().BeEmpty();
            _actor.AsksFor(SelectedState.Of(SubmissionPage.PrivacyPolicy)).Should().BeFalse();
        }

        [Given(@"all necessary fields except the name are filled out")]
        public void GivenAllNecessaryFieldsExceptTheNameAreFilledOut()
        {
            var submissionEntryFormRowObjects = new List<SubmissionEntryFormRowObject>
            {
                new("Url", "https://example.org"),
                new("Type", "Blog Posts"),
                new("Email", "someone@example.org"),
                new("Description", "something really cool")
            };

            _actor.AttemptsTo(FillOutSubmissionForm.With(submissionEntryFormRowObjects));
            _actor.AttemptsTo(Click.On(SubmissionPage.PrivacyPolicy));
        }

        [When(@"the name '(.*)' is provided")]
        public void WhenTheNameIsProvided(string name)
        {
            _actor.AttemptsTo(SendKeys.To(SubmissionPage.NameField, name));
        }

        [When(@"the name stays empty")]
        public void WhenTheNameStaysEmpty()
        {
            _actor.AttemptsTo(SendKeys.To(SubmissionPage.NameField, string.Empty));
        }

        [Given(@"the content suggestion page is opened and filled with")]
        public void GivenTheContentSuggestionPageIsOpenedAndFilledWith(Table table)
        {
            _actor.AttemptsTo(Navigate.ToUrl(ConfigurationProvider.BaseAddress));
            GivenTheFilledOutSubmissionEntryForm(table);
        }

        [Given(@"a filled content suggestion page is opened")]
        public void GivenAFilledContentSuggestionPageIsOpened()
        {
            _actor.AttemptsTo(Navigate.ToUrl(ConfigurationProvider.BaseAddress));
            GivenTheSubmissionEntryFormIsFilled();
        }
    }
}