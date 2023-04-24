use std::rc::Rc;
use std::vec;

use crate::errors::ArgumentError;
use crate::float2::Float2;

#[derive(Debug)]
pub struct Polygon {
    pub vertexes: Vec<Rc<Float2>>, // sorted CCW
}

#[derive(Debug)]
pub struct Polygonization {
    pub vertexes: Vec<Rc<Float2>>,
    pub polygons: Vec<Polygon>,
}

impl Polygonization {
    // pub fn as_voronoi(
    //     seeds: Vec<Float2>
    // ) -> Polygonization {
    // }

    pub fn as_squares(size: usize) -> Result<Polygonization, ArgumentError> {
        if size < 1 {
            return Err(ArgumentError::new("squares per side must be at least 1"));
        }
        let mut polygonization: Polygonization = Polygonization {
            vertexes: Vec::new(),
            polygons: Vec::new(),
        };
        for row in 0..size + 1 {
            for col in 0..size + 1 {
                polygonization.vertexes.push(Rc::new(Float2 {
                    x: (col as f32) / (size as f32),
                    y: (row as f32) / (size as f32),
                }));
            }
        }
        for row in 0..size {
            for col in 0..size {
                polygonization.polygons.push(Polygon {
                    vertexes: vec![
                        polygonization.vertexes[(size + 1) * row + (col + 1)].clone(),
                        polygonization.vertexes[(size + 1) * row + col].clone(),
                        polygonization.vertexes[(size + 1) * (row + 1) + col].clone(),
                        polygonization.vertexes[(size + 1) * (row + 1) + (col + 1)].clone(),
                    ],
                });
            }
        }
        Ok(polygonization)
    }
}
