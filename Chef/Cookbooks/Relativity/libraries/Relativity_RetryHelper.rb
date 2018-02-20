class Chef::Recipe::RetryHelper

  include Chef::Mixin::ShellOut

  def self.execute_rest_call(http_obj, request, max_retry, err_msg)
    retry_cnt = 0
    maximum_retry = max_retry
    result = nil
    error = nil
    success = false

    while retry_cnt < maximum_retry && success == false
      begin
        error = nil
        result = http_obj.request(request)
        if result.is_a?(Net::HTTPOK) || result.is_a?(Net::HTTPSuccess)
          success = true
        elsif result.is_a?(Net::HTTPUnauthorized)
          error = err_msg + ' ' + 'Unauthorized Request, double check credentials'
        else
          error = err_msg + ' ' + result.message + ' ' + result.body
        end
        rescue Exception => e
          error = e.to_s
      end
      retry_cnt += 1
    end

    if result && error.nil?
      result
    elsif error
      raise error
    end
  end
end
