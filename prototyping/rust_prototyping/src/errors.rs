use std::{fmt, error::Error};



#[derive(Debug)]
pub struct ArgumentError {
    details: String
}

impl ArgumentError {
    pub fn new(msg: &str) -> ArgumentError {
        ArgumentError{details: msg.to_string()}
    }
}

impl fmt::Display for ArgumentError {
    fn fmt(&self, f: &mut fmt::Formatter) -> fmt::Result {
        write!(f,"{}",self.details)
    }
}

impl Error for ArgumentError {
    fn description(&self) -> &str {
        &self.details
    }
}