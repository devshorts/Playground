require 'rspec'
require 'pry'
require 'pry-stack_explorer'
require 'pry-nav'
require_relative '../lib/runner'
require_relative '../lib/single'
include Runner
include Ones

describe 'My behaviour' do

  context 'balls' do

    it 'should do something' do
      expect(true).to eq(true)
    end

    it 'should be a singleton' do
      s = Single.new

      apply = lambda do |fn, x, y|
        x.send(fn, y)
      end

      mult = apply.curry.call(:*)

      puts mult.(1, 2)
    end

  end

end