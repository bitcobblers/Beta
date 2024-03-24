﻿namespace Beta;

/// <summary>
///     Defines an exception that is thrown whenever test suite activation fails.
/// </summary>
/// <param name="message">The reason for the failure.</param>
public class TestContainerActivationFailedException(string message) : Exception(message);