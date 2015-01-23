require 'rspec'
require 'pry'
require 'pry-stack_explorer'
require 'pry-nav'
require_relative '../lib/single'
require 'hashie'
require 'json'
include Hashie

class HashTest
  def initialize(test: "biz", biz: "boo", **opts)
    puts test
    puts biz
    puts opts
  end
end
class Test < Trash
  property :foo
  property :biz
end

describe 'My behaviour' do

  context 'balls' do

    it 'should do something' do
      HashTest.new(:test => 'testz', :biz => 'baz', :shiz => "shaz")
    end

  end

end